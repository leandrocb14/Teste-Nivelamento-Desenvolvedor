namespace Questao5.Domain.Entities;

public class Idempotency
{
    public string Key { get; private set; }
    public string Request { get; private set; }
    public string Result { get; private set; }


    public Idempotency(string key, string request, string result)
    {
        Key = key;
        Request = request;
        Result = result;
    }
}
