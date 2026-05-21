import { useCookies } from 'react-cookie';
import { jwtDecode } from 'jwt-decode';

export const useAuth = () => {
  const [cookies, setCookie, removeCookie] = useCookies(['jwt_token']);
  const token = cookies.jwt_token;

  let user = null;
  if (token) {
    try {
      user = jwtDecode(token);
    } catch (err) {
      console.error("Token invalide");
    }
  }

  const logout = () => {
    removeCookie('jwt_token', { path: '/' });
  };

  return {
    token,                  
    user,                   
    isAuthenticated: !!token,
    logout                  
  };
};