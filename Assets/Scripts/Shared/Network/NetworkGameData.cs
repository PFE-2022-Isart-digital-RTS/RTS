using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Netcode;

public enum EDataHeader
{
    MoveTo // Data : Vector3
}

public struct NetworkGameData : INetworkSerializable
{
    internal struct BinaryData
    {
        public byte[] dataByte;

        public BinaryData(object data)
        {
            dataByte = SerializeData(data);
        }
    
        private static byte[] SerializeData(object obj)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    
        public object DeserializeData()
        {
            var binaryFormatter = new BinaryFormatter{};

            using (var memoryStream = new MemoryStream(dataByte))
                return binaryFormatter.Deserialize(memoryStream);
        }
    }
    
    private BinaryData dataByte;
    
    public EDataHeader header;
    public object obj;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            dataByte = new BinaryData(obj);
        }
        
        serializer.SerializeValue(ref header);
        serializer.SerializeValue(ref dataByte.dataByte);
        
        if (serializer.IsReader)
        {
            obj = dataByte.DeserializeData();
        }
    }
}
