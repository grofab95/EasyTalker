export default interface ApiResponse<T> {
    isSuccess: string
    error: string
    data: T
}