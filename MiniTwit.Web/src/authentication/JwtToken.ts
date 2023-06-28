import jwtDecode from "jwt-decode"

export function getUserId(): string {
    return ""
}

export function getUsername(): string {
    return ""
}

export function isLoggedIn(): boolean {
    return true
}

export function parseToken(token: string) {
    const decodedToken = jwtDecode<any>(token)
    console.log(decodedToken.nameid)
    console.log(decodedToken.unique_name)
    console.log(decodedToken.email)
}