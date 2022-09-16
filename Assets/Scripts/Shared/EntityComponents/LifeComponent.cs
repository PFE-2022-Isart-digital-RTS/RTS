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

    public UnityEvent OnNoLife;
    public UnityEvent OnFullLife;

    [SerializeField]
    GameObject healthBar;

    public float lifeRatio { get { return life / maxLife; } }

    public enum State
    { 
        IsFullLife,
        IsDamaged,
        IsDead
    }

    public State state { private set; get; }

    //public override void OnNetworkSpawn()
    public void Start()
    {
    //    base.OnNetworkSpawn();
    //}
    //void Start()
    //{
        UpdateState();
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsClient)
        {
            GameObject progressBarGO = Instantiate(healthBar, gameObject.transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, transform);
            IProgressBar progressBar = progressBarGO.GetComponent<IProgressBar>();
            progressBar.value = lifeRatio;
            repLife.OnValueChanged += (float oldValue, float newValue) =>
            {
                progressBar.value = lifeRatio;
            };

            if (Camera.main != null) 
                progressBarGO.transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
        }
    }

    public void UpdateState()
    {
        if (life <= 0f)
        {
            state = State.IsDead;
        }
        else if (life >= maxLife)
        {
            state = State.IsFullLife;
        }
        else 
            state = State.IsDamaged;
    }

    public void DealMeleeDamages(WeaponComponent attacker, float nbDamages)
    {
        DealDamages(nbDamages);
    }

    public void DealDamages(float damages)
    {
        life -= damages;
        if (state != State.IsDead && life <= 0f)
        {
            life = 0f;
            state = State.IsDead;
            OnNoLife?.Invoke();
            Destroy(gameObject); 
        }
        else
            state = State.IsDamaged;
    }

    public void Heal(float nbLifeAdded)
    {
        life += nbLifeAdded;
        if (life >= maxLife)
        {
            life = maxLife;
            state = State.IsFullLife;
            OnFullLife?.Invoke();
        }
        else
            state = State.IsDamaged;
    }
}
