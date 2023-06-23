import axios from "axios";

const PRODUCTION_BASE_URL = "http://127.0.0.1:5000/"
const DEVELOPMENT_BASE_URL = "http://127.0.0.1:5000/"
const API_BASE_URL = process.env.NODE_ENV === 'development' ? DEVELOPMENT_BASE_URL : PRODUCTION_BASE_URL

const axiosClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
})

export default axiosClient