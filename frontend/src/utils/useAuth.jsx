import { useCookies } from 'react-cookie';
import { jwtDecode } from 'jwt-decode';
import { useState, useEffect, useCallback } from 'react';

export const useAuth = () => {
    const [cookies, setCookie, removeCookie] = useCookies(['jwt_token']);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const token = cookies.jwt_token;

    // Récupérer les détails utilisateur depuis l'API
    const fetchUserDetails = useCallback(async (userId) => {
        if (!userId) return;

        try {
            const response = await fetch(`/api/User/${userId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            if (response.ok) {
                const userData = await response.json();
                setUser(prev => ({ ...prev, ...userData }));
                localStorage.setItem('user', JSON.stringify(userData));
            }
        } catch (error) {
            console.error('Erreur récupération user:', error);
        }
    }, [token]);

    useEffect(() => {
        if (token) {
            try {
                const decoded = jwtDecode(token);
                const userId = decoded.userId ;

                if (!userId) {
                    console.error('Aucun ID trouvé dans le token. Clés disponibles:', Object.keys(decoded));
                }

                setUser(decoded);

                // Récupérer les infos complètes APRÈS avoir décodé
                if (userId) {
                    fetchUserDetails(userId);
                }
            } catch (err) {
                console.error("Token invalide:", err);
                setUser(null);
            }
        } else {
            setUser(null);
        }
        setLoading(false);
    }, [token, fetchUserDetails]);

    const logout = () => {
        removeCookie('jwt_token', { path: '/' });
        setUser(null);
        localStorage.removeItem('user');
    };

    return {
        token,
        user,
        loading,
        isAuthenticated: !!token,
        logout
    };
};