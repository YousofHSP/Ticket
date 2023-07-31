using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Common
{
    public interface IEntity
    {

    }
    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
        DateTime CreatedAt { get; set; }
    }
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}