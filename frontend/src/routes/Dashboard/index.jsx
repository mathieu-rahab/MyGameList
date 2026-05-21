import { Link } from "react-router";
import {useTranslation} from "react-i18next";
import "./index.css"
import {useUserService} from "../../api/userService.js";
import {useEffect, useState} from "react";
import {useAuth} from "../../utils/useAuth.jsx";


export default function Dashboard (){
    const {t, i18n} = useTranslation();
    const { getRecentGames } = useUserService();
    const { user, loading: authLoading } = useAuth();
    const [recentGames, setRecentGames] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [imageErrors, setImageErrors] = useState({});

    const handleImageError = (gameId) => {
        setImageErrors(prev => ({ ...prev, [gameId]: true }));
    };

    useEffect(() => {
        const fetchRecentGames = async () => {
            try {
                setLoading(true);
                // Récupérer l'ID depuis user
                const userId = user?.userId;
                if (!userId) {
                    setError('User ID not found');
                    setLoading(false);
                    return;
                }
                const games = await getRecentGames(userId, 6);
                setRecentGames(games);
                setError(null);
            } catch (err) {
                if (err.error) {
                    setError(err.error);
                }
                else {
                    setError(err.status);
                }
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        // Attendre que l'user soit chargé avant de récupérer les jeux
        if (!authLoading && user?.userId) {
            fetchRecentGames();
        }
    }, [user?.userId, authLoading]);



    const renderGameRows = () => {
        if (loading) return <div>Chargement...</div>;
        if (error) return <div>Erreur: {error}</div>;

        return recentGames.map((game) => (
            <div key={game.id} className="game-row">
                <div className="gthumb gt1">
                    {imageErrors[game.id] ? (
                        <i className="ti ti-device-gamepad-2"></i>
                    ) : (
                        <img
                            src={game.image}
                            alt={game.name}
                            onError={() => handleImageError(game.id)}
                            style={{width: '100%', height: '100%', objectFit: 'cover'}}
                        />
                    )}
                </div>
                <div className="ginfo">
                    <div className="gname">{game.name}</div>
                    <div className="gmeta">
                        {Math.round(game.playtime2Weeks / 60)}h joué
                    </div>
                    <div className="prog">
                        <div
                            className="prog-b"
                            style={{'--progress-width': game.achievementProgression ? `${game.achievementProgression}%` : '0%', background: '#a78bfa'}}
                        >
                        </div>
                    </div>
                </div>
                <div className="gtime">
                    {Math.round(game.playtimeForever / 60)}h<br />
                    total
                </div>
            </div>
        ));
    };

    return (
        <main className="dashbord">
            <div className="greeting">
                <div className="greeting-sub">Lundi 27 avril 2026</div>
                <h1>Bon retour, <span>{user?.pseudo || 'Guest'}</span></h1>
            </div>

            <div className="stats-row">
                <div className="scard glass">
                    <div className="scard-label">Jeux joués</div>
                    <div className="scard-val">47</div>
                    <div className="scard-sub">+3 ce mois</div>
                </div>
                <div className="scard glass">
                    <div className="scard-label">Trophées</div>
                    <div className="scard-val">312</div>
                    <div className="scard-sub">8 platines</div>
                </div>
                <div className="scard glass">
                    <div className="scard-label">Collections</div>
                    <div className="scard-val">5</div>
                    <div className="scard-sub">124 éléments</div>
                </div>
                <div className="scard glass">
                    <div className="scard-label">Amis en ligne</div>
                    <div className="scard-val">6</div>
                    <div className="scard-sub">sur 28 amis</div>
                </div>
            </div>

            <div className="grid2">
                <div className="section glass">
                    <div className="sec-head">
                        <span className="sec-title">Jeux récents</span>
                        <span className="sec-link">voir tout</span>
                    </div>
                    {renderGameRows()}
                </div>

                <div className="section glass">
                    <div className="sec-head"><span className="sec-title">Trophées récents</span><span className="sec-link">voir
                tout</span>
                    </div>
                    <div className="tlist">
                        <div className="trow">
                            <div className="tico ti-pl">◆</div>
                            <div>
                                <div className="tname">Maître de l'univers</div>
                                <div className="tgame">Stellar Odyssey</div>
                            </div>
                            <span className="tbadge b-pl">PLATINE</span>
                        </div>
                        <div className="trow">
                            <div className="tico ti-go">◇</div>
                            <div>
                                <div className="tname">Survivant</div>
                                <div className="tgame">Crimson Arena</div>
                            </div>
                            <span className="tbadge b-go">OR</span>
                        </div>
                        <div className="trow">
                            <div className="tico ti-si">○</div>
                            <div>
                                <div className="tname">Gardien de forêt</div>
                                <div className="tgame">Verdant Realm</div>
                            </div>
                            <span className="tbadge b-si">ARGENT</span>
                        </div>
                        <div className="trow">
                            <div className="tico ti-br">◌</div>
                            <div>
                                <div className="tname">Navigateur</div>
                                <div className="tgame">Deep Protocol</div>
                            </div>
                            <span className="tbadge b-br">BRONZE</span>
                        </div>
                    </div>
                </div>
            </div>

            <div className="grid3">
                <div className="section glass">
                    <div className="sec-head"><span className="sec-title">Collections</span><span className="sec-link">gérer</span>
                    </div>
                    <div className="col-row">
                        <div className="ccard">
                            <div className="cico">◈</div>
                            <div className="cname">SF & Espace</div>
                            <div className="ccnt">12 jeux</div>
                        </div>

                        <div className="ccard cadd">
                            <div className="cplus">+</div>
                            <div className="cname">Nouvelle</div>
                        </div>
                    </div>
                </div>

                <div className="section glass">
                    <div className="sec-head"><span className="sec-title">Amis</span><span className="sec-link">voir tout</span>
                    </div>
                    <div className="frow">
                        <div className="fav on" >MK</div>
                        <div>
                            <div className="fname">MaxKnight</div>
                            <div className="fst">Stellar Odyssey</div>
                        </div>
                        <div className="flvl">lv.72</div>
                    </div>
                </div>
            </div>
        </main>
    );
}