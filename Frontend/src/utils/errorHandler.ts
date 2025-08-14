import { AxiosError } from "axios";

export const handleApiError = (error: unknown): string => {
    let errorMessage = "Неизвестная ошибка";

    if (error instanceof AxiosError) {
        if (error.response) {
            errorMessage = error.response.data?.message
                || error.response.data?.error
                || JSON.stringify(error.response.data)
                || "Ошибка не определена";
        } else if (error.request) {
            errorMessage = "Сервер не ответил";
        } else {
            error.message = error.message || "Ошибка при запросе";
        }
    } else if (error instanceof Error) {
        errorMessage = error.message;
    }

    return errorMessage;
}