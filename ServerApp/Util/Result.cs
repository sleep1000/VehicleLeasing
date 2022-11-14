namespace VehicleLeasing.Util
{
    public class Result<T, E> where E : Exception?
    {
        private T? value = default(T);
        private E? error = null;
        private readonly bool hasValue;

        public bool IsSome => hasValue;

        public bool IsError => !hasValue;

        public T Value
        {
            get
            {
                if (!hasValue) throw new NullReferenceException();
                return value!;
            }
        }

        public E Error
        {
            get
            {
                if (hasValue) throw new NullReferenceException();
                return error!;
            }
        }

        public static Result<T, E> Some(T value) => new Result<T, E>(value);

        public static Result<T, E> Err(E error) => new Result<T, E>(error);

        private Result(T value)
        {
            this.value = value;
            hasValue = true;
        }

        private Result(E error)
        {
            this.error = error;
        }

        public TResult Match<TResult>(Func<T, TResult> someFunc, Func<TResult> noneFunc) =>
            hasValue ? someFunc(value!) : noneFunc();

        public override bool Equals(object? obj)
        {
            if (obj is Result<T, E> opt)
            {
                return hasValue
                    ? opt.IsSome && opt.value!.Equals(value)
                    : opt.IsError && opt.error!.Equals(error);
            }
            return false;
        }

        public override int GetHashCode() =>
            hasValue ? value!.GetHashCode() : error!.GetHashCode();
    }
}
