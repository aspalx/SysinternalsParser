namespace ListDllsParser.Dsl
{
    public interface IParser<in TInput, out TOutput>
    {
        TOutput Parse(TInput input);
    }
}