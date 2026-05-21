import { createContext, useContext, useState, useEffect } from 'react';
import { useCookies } from 'react-cookie';

const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [cookies] = useCookies(['jwt_token']);

    useEffect(() => {
        // Au montage, récupérer les infos utilisateur si connecté
        if (cookies.jwt_token) {
            fetchUserInfo();
        } else {
            setLoading(false);
        }
    }, [cookies.jwt_token]);

    const fetchUserInfo = async () => {
        try {
            const response = await fetch('/api/User/me', {
                headers: {
                    'Authorization': `Bearer ${cookies.jwt_token}`
                }
            });
            if (response.ok) {
                const userData = await response.json();
                setUser(userData);
                // Optionnel : stocker pour la réhydratation au rechargement
                localStorage.setItem('user', JSON.stringify(userData));
            }
        } catch (error) {
            console.error('Erreur récupération user:', error);
            setUser(null);
        } finally {
            setLoading(false);
        }
    };

    const logout = () => {
        setUser(null);
        localStorage.removeItem('user');
    };

    return (
        <AuthContext.Provider value={{ user, loading, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};