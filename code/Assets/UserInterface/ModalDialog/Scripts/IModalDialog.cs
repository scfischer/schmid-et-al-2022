namespace UserInterface.ModalDialog
{
    public interface IModalDialog
    {
        ModalDialogController.DialogCallback Callback { get; set; }
    }
    
    public interface IModalDialog<TReturnValue>
    {
        ModalDialogController.DialogCallback<TReturnValue> Callback { get; set; }
    }
}