namespace P2P.Domains.Entities;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    
    public static ApiResponse<T> SuccessResponse (T data, string message = null)=> new ApiResponse<T> { Success = true, Message = message, Data = data };
    
    public static ApiResponse<T> FailedResponse (T data, string message = null) => new ApiResponse<T> { Success = false, Message = message, Data = data };
    
}