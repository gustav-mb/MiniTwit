import { LoginDTO } from "../models/LoginDTO"
import { TokenDTO } from "../models/TokenDTO"
import axiosClient from "../utilities/Axios"

const ROUTE = 'Authentication'

class AuthenticationService {
    public async login(username: string, password: string): Promise<TokenDTO> {
        const loginDTO: LoginDTO = {
            username: username,
            password: password
        }
        
        return await axiosClient.post(`${ROUTE}/login`, loginDTO)
    }

    // TODO
    public async refreshToken(): Promise<TokenDTO> {
        const tokenDTO: TokenDTO = {
            accessToken: "",
            refreshToken: ""
        }

        return await axiosClient.post(`${ROUTE}/refresh-token`, tokenDTO)
    }
}

export default AuthenticationService