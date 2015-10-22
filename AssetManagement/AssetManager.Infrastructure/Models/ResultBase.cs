using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models
{
    public abstract class ResultBase<T>
    {
        public List<string> Errors { get; set; }

        public T Result { get; set; }
    }
}
