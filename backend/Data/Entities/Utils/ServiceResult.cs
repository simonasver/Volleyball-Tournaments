namespace Backend.Data.Entities.Utils;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public int? ErrorStatus { get; set; }
    public string? ErrorMessage { get; set; }

    public ServiceResult()
    {
        IsSuccess = true;
    }

    public ServiceResult(int errorStatus, string errorMessage)
    {
        IsSuccess = false;
        ErrorStatus = errorStatus;
        ErrorMessage = errorMessage;
    }

    public static ServiceResult Success()
    {
        return new ServiceResult();
    }

    public static ServiceResult Failure(int errorStatus, string errorMessage)
    {
        return new ServiceResult(errorStatus, errorMessage);
    }
}
public class ServiceResult<T>: ServiceResult
{
    public T? Data { get; set; }
    
    public ServiceResult(T data)
    {
        IsSuccess = true;
        Data = data;
    }
    
    public static ServiceResult<T> SuccessWithData(T data)
    {
        return new ServiceResult<T>(data);
    }
}