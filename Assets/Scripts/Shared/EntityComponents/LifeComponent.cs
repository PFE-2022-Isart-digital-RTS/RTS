using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public interface ICanBeAttacked
{
    void OnAttacked(WeaponComponent attacker, float nbDamages);
}

public class LifeComponent : NetworkBehaviour
{
    [SerializeField]
    NetworkVariable<float> repLife = new NetworkVariable<float>(10f);

    float life { get => repLife.Value; set => repLife.Value = value; }

    [SerializeField]
    float maxLife = 10;

    [SerializeField]
    bool shouldDestroyOnNoLife = true;

    public UnityEvent OnNoLife;
    public UnityEvent OnFullLife;
    public UnityEvent<WeaponComponent, float> OnAttacked;
    public UnityEvent<WeaponComponent, float> OnKilled;

    [SerializeField]
    GameObject healthBar;

    public float LifeRatio { get { return life / maxLife; } }
    public float Life 
    { 
        get => life;  
        set
        {
            float diff = value - Life;
            if (diff < 0)
                DealDamages(-diff);
            else
                Heal(diff);
        }
    }
    public float MaxLife { get => maxLife; }

    public enum LifeState
    { 
        IsFullLife,
        IsDamaged,
        IsDead
    }

    public LifeState State { private set; get; }

    public void Start()
    {
        UpdateState();
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsClient)
        {
            GameObject progressBarGO = Instantiate(healthBar, gameObject.transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, transform);
            IProgressBar progressBar = progressBarGO.GetComponent<IProgressBar>();
            progressBar.value = LifeRatio;
            repLife.OnValueChanged += (float oldValue, float newValue) =>
            {
                progressBar.value = LifeRatio;
            };

            if (Camera.main != null) 
                progressBarGO.transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
        }
    }

    public void UpdateState()
    {
        if (life <= 0f)
        {
            State = LifeState.IsDead;
        }
        else if (life >= maxLife)
        {
            State = LifeState.IsFullLife;
        }
        else 
            State = LifeState.IsDamaged;
    }

    public void DealMeleeDamages(WeaponComponent attacker, float nbDamages)
    {
        if (State == LifeState.IsDead)
            return;

        DealDamages(nbDamages);
        OnAttacked?.Invoke(attacker, nbDamages);
        if (State == LifeState.IsDead)
            OnKilled?.Invoke(attacker, nbDamages);
    }

    // damages : Positive value only
    public void DealDamages(float damages)
    {
        life -= damages;
        if (life <= 0f)
        {
            life = 0f;
            if (State != LifeState.IsDead)
            {
                State = LifeState.IsDead;
                OnNoLife?.Invoke();
                if (shouldDestroyOnNoLife)
                    Destroy(gameObject);
            }
        }
        else
            State = LifeState.IsDamaged;
    }

    // nbLifeAdded : Positive value only
    public void Heal(float nbLifeAdded)
    {
        life += nbLifeAdded;
        if (life >= maxLife)
        {
            life = maxLife;
            State = LifeState.IsFullLife;
            OnFullLife?.Invoke();
        }
        else
            State = LifeState.IsDamaged;
    }
}
