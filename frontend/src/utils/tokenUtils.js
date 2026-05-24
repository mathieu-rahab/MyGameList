const COOKIE_OPTIONS = { path: '/', maxAge: 3600, secure: true, sameSite: 'strict' };

export function shouldRenew(token) {
    try {
        const { exp } = JSON.parse(atob(token.split('.')[1]));
        console.log(exp);
        console.log(exp * 1000 - Date.now())
        console.log(15 * 60 * 1000);
        return exp * 1000 - Date.now() < 15 * 60 * 1000; // moins de 15 min restantes
    } catch {
        return false;
    }
}


export function storeToken(setCookie, token) {
    setCookie('jwt_token', token, COOKIE_OPTIONS);
}