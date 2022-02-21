using System.Collections.Generic;

namespace Contoso.DigitalGoods.Application.Models
{
    public interface IMessage<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public interface IObjectCollectionMessage<T>
    {
        public bool Success { get; set; }
        public ICollection<T> Data { get; set; }
        public string Message { get; set; }
    }
}
