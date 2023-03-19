import { User } from "../store/auth-slice"

export function isAdmin(user: User | undefined): boolean {
    if(user === undefined) return false;
    return user.roles?.includes("admin") ?? false;
}