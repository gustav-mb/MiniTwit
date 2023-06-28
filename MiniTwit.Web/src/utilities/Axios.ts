import axios from "axios";
import { APIError } from "../models/APIError";

const PRODUCTION_BASE_URL = "http://127.0.0.1:5000/"
const DEVELOPMENT_BASE_URL = "http://127.0.0.1:5000/"
const API_BASE_URL = process.env.NODE_ENV === 'development' ? DEVELOPMENT_BASE_URL : PRODUCTION_BASE_URL

const axiosClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
})

axiosClient.interceptors.response.use(response => {
    return response.data
},
(error: Error) => {
    if (axios.isAxiosError(error))
        return Promise.reject(error.response?.data)

    const APIError: APIError = {
        errorMsg: error.message,
        status: -1
    } 

    return Promise.reject(APIError)
})

export default axiosClient