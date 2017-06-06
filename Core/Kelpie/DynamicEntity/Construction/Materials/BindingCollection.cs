using System.Collections.Generic;

namespace Kelpie.DynamicEntity.Construction.Materials
{
    /// <summary>
    /// Binds to an ICollection of an other type
    /// </summary>
    /// <typeparam name="TOutput">Destination</typeparam>
    /// <typeparam name="TInput">Source</typeparam>
    public class BindingCollection<TOutput, TInput> : ICollection<TInput> where TOutput : class, TInput
    {
        ICollection<TOutput> collection;

        public BindingCollection(ICollection<TOutput> collection)
        {
            this.collection = collection;
        }

        public void Add(TInput item)
        {
            collection.Add((TOutput)item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(TInput item)
        {
            return collection.Contains((TOutput)item);
        }

        public void CopyTo(TInput[] array, int arrayIndex)
        {
            TOutput[] tpmArray = new TOutput[array.Length];
            collection.CopyTo(tpmArray, arrayIndex);

            for (int i = arrayIndex; i < array.Length - arrayIndex; ++i)
                array[i] = tpmArray[i];
        }

        public int Count
        {
            get
            {
                return collection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return collection.IsReadOnly;
            }
        }

        public bool Remove(TInput item)
        {
            return collection.Remove((TOutput)item);
        }

        public IEnumerator<TInput> GetEnumerator()
        {
            return (IEnumerator<TInput>)collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}
