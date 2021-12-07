
namespace Test.Control 
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(DialogueController callingController);
    }
}