namespace WinTail.Messages.Error
{
    /// <summary>
    /// User provided no input
    /// </summary>
    public class NullInputError : InputError
    {
        public NullInputError(string reason)
            : base(reason)
        {
        }
    }
}