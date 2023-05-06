namespace Backend.Data.Entities.Utils;

public class ServiceResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public int ErrorStatus { get; set; } = StatusCodes.Status500InternalServerError;
    public string? ErrorMessage { get; set; }

    public ServiceResult()
    {
        IsSuccess = true;
    }

    private ServiceResult(T data)
    {
        IsSuccess = true;
        Data = data;
    }

    private ServiceResult(int errorStatus, string errorMessage)
    {
        IsSuccess = false;
        ErrorStatus = errorStatus;
        ErrorMessage = errorMessage;
    }

    public static ServiceResult<T> Success()
    {
        return new ServiceResult<T>();
    }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>(data);
    }

    public static ServiceResult<T> Failure(int errorStatus, string errorMessage = "")
    {
        return new ServiceResult<T>(errorStatus, errorMessage);
    }
}