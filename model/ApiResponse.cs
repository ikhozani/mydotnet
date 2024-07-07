namespace FileUploadSvc.model;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public ApiResponse(int code, string status, string message, T data)
    {
        Code = code;
        Status = status;
        Message = message;
        Data = data;
    }
}