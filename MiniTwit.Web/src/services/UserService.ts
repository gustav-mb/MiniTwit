import { UserCreateDTO } from "../models/UserDTO"
import axiosClient from "../utilities/Axios"

const ROUTE = 'User'

class UserService {
    public async register(username: string, password: string, email: string): Promise<undefined> {
        const userCreateDTO: UserCreateDTO = {
            username: username,
            password: password,
            email: email
        }

        return await axiosClient.post(`${ROUTE}/register`, userCreateDTO)
    }

    public async logout(): Promise<undefined> {
        return await axiosClient.post(`${ROUTE}/logout`)
    }
}

export default UserService