namespace CDK.Assets
{
    public interface IBindingCloneable<T>
    {
        T Clone(bool binding);
    }
}
