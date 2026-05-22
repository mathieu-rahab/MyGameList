import { Link } from "react-router";
import {useTranslation} from "react-i18next";
import "./index.css"
import {useUserService} from "../../api/userService.js";
import {useEffect, useState} from "react";
import {useAuth} from "../../utils/useAuth.jsx";


export default function Dashboard (){
    const {t, i18n} = useTranslation();
    const { getRecentGames, getRecentAchievements, getProgressionGame } = useUserService();
    const { user, loading: authLoading } = useAuth();
    const [recentGames, setRecentGames] = useState([]);
    const [recentAchievements, setRecentAchievements] = useState([]);
    const [progressions, setProgressions] = useState({}); // clé: gameId, valeur: progression
    const [loadingRecentGames, setLoadingRecentGames] = useState(true);
    const [loadingRecentAchiev, setLoadingloadingRecentAchiev] = useState(true);

    const [error, setError] = useState(null);
    const [imageErrors, setImageErrors] = useState({});

    const handleImageError = (gameId) => {
        setImageErrors(prev => ({ ...prev, [gameId]: true }));
    };

    const getAchievementRarity = (percent) => {
        if (percent <= 10) return { rarity: 'platinum', label: t('achievements.platinum'), class: 'b-pl' };
        if (percent <= 30) return { rarity: 'silver', label: t('achievements.silver'), class: 'b-si' };
        if (percent <= 60) return { rarity: 'gold', label: t('achievements.gold'), class: 'b-go' };

        return { rarity: 'bronze', label: t('achievements.bronze'), class: 'b-br' };
    };

    // ... existing code ...

    useEffect(() => {
        const fetchRecentGames = async () => {
            try {
                setLoadingRecentGames(true);
                setLoadingloadingRecentAchiev(true);
                const userId = user?.userId;
                if (!userId) {
                    setError('User ID not found');
                    setLoadingRecentGames(false);
                    return;
                }
                const games = await getRecentGames(
                    userId,
                    6,
                    false,
                    i18n.language === 'fr' ? 'french' : 'english'
                );
                setRecentGames(games);
                setError(null);

                // Récupérer les progressions en parallèle
                const lang = i18n.language === 'fr' ? 'french' : 'english';
                games.forEach(game => {
                    console.log(game);
                    const progressionLink = game.links?.find(
                        link => link.rel === 'get-user-progression-game'
                    );
                    if (progressionLink) {
                        getProgressionGame(progressionLink.href, lang)
                            .then(progressionData => {
                                setProgressions(prev => ({
                                    ...prev,
                                    [game.id]: progressionData.progression
                                }));
                            })
                            .catch(err => {
                                console.error(`Erreur progression pour ${game.name}:`, err);
                            });
                    }
                });
            } catch (err) {
                if (err.error) {
                    setError(err.error);
                } else {
                    setError(err.status);
                }
                console.error(err);
            } finally {
                setLoadingRecentGames(false);
                setLoadingloadingRecentAchiev(false);
            }
        };

        const fetchRecentAchievements = async () => {
            try {
                const userId = user?.userId;
                if (!userId) return;

                const achievements = await getRecentAchievements(
                    userId,
                    7,
                    true,
                    i18n.language === 'fr' ? 'french' : 'english'
                );
                setRecentAchievements(achievements);
            } catch (err) {
                console.error('Erreur lors de la récupération des trophées:', err);
            }
        };

        if (!authLoading && user?.userId) {
            fetchRecentGames()
                .then(fetchRecentAchievements);
        }
    }, [user?.userId, authLoading, i18n.language]);




    const renderGameRows = () => {
        if (loadingRecentGames) {
            return <div className="message">{t('Dashboard.loading')}</div>;
        }
        if (error) {
            return <div className="message">{t('Dashboard.error')}</div>;
        }
        if(recentGames.length === 0) {
            return <div className="message">{t('Dashboard.NoRecentGames')}</div>;
        }

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
                            style={{
                                '--progress-width': progressions[game.id] !== undefined
                                    ? `${progressions[game.id]}%`
                                    : '0%',
                                background: '#a78bfa'
                            }}
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

    const renderAchievementRows = () => {
        if (loadingRecentAchiev) {
            return <div className="message">{t('Dashboard.loading')}</div>;
        }
        if (error) {
            return <div className="message">{t('Dashboard.error')}</div>;
        }
        if(recentAchievements.length === 0) {
            return <div className="message">{t('Dashboard.NoRecentAchievements')}</div>;
        }

        return recentAchievements.map((achievement, index) => {
            const rarityInfo = getAchievementRarity(achievement.rarity);
            return (
                <div key={index} className="trow">
                    <div>
                        <img className="tico achievementIcon" src={achievement.icon} alt={achievement.displayName}></img>
                    </div>
                    <div>
                        <div className="tname">{achievement.displayName}</div>
                        <div className="tgame">{achievement.gameName}</div>
                    </div>
                    <span className={`tbadge ${rarityInfo.class}`}>
                        {rarityInfo.label.toUpperCase()}
                    </span>
                </div>
            );
        });
    };


    return (
        <main className="dashbord">
            <div className="greeting">
                <div className="greeting-sub">
                    {new Date().toLocaleDateString(i18n.language, { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })}
                </div>
                <h1>{t('Dashboard.WelcomeBack')}<span> {user?.pseudo || 'Guest'}</span></h1>
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
                        <span className="sec-title">{t('Dashboard.RecentGames')}</span>
                        <span className="sec-link">{t('Dashboard.ShowMore')}</span>
                    </div>
                    {renderGameRows()}
                </div>

                <div className="section glass">
                    <div className="sec-head"><span className="sec-title">{t('Dashboard.RecentAchievements')}</span>
                        <span className="sec-link">{t('Dashboard.ShowMore')}</span>
                    </div>
                    <div className="tlist">
                        {renderAchievementRows()}

                    </div>
                </div>
            </div>

            <div className="grid3">
                <div className="section glass">
                    <div className="sec-head"><span className="sec-title">Collections</span>
                        <span className="sec-link">{t('Dashboard.Manage')}</span>
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
                    <div className="sec-head"><span className="sec-title">{t('Dashboard.Friends')}</span>
                        <span className="sec-link">{t('Dashboard.ShowMore')}</span>
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