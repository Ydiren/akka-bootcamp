namespace WinTail.Messages.Error
{
    /// <summary>
    /// User provided invalid input
    /// </summary>
    public class ValidationError : InputError
    {
        public ValidationError(string reason)
            : base(reason)
        {
        }
    }
}