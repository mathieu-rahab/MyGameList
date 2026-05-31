import { useEffect, useState } from "react";
import {Link, useParams} from "react-router-dom";
import { useTranslation } from "react-i18next";
import "./index.css";
import { useUserService } from "../../api/userService.js";
import { useAuth } from "../../utils/useAuth.jsx";
import {useNavigate} from 'react-router-dom';
import CollectionModal from "../../components/CreateCollection/CollectionModal.jsx";


export default function Collection() {
    const { t, i18n } = useTranslation();
    const { collectionId } = useParams();
    const navigate = useNavigate();

    const { user } = useAuth();
    const { getOneCollection, searchGames, addGameCollection, removeGameCollection, getGameInfo, deleteCollection, updateCollection } = useUserService();

    const [collection, setCollection] = useState(null);
    const [games, setGames] = useState([]);
    const [searchResults, setSearchResults] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [searchLoading, setSearchLoading] = useState(false); // Nouvel état pour le chargement de la recherche
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [showEditModal, setShowEditModal] = useState(false);
    const [imageErrors, setImageErrors] = useState({});

    const handleImageError = (gameId) => {
        setImageErrors(prev => ({ ...prev, [gameId]: true }));
    };

    // Fetch collection details
    useEffect(() => {
        const fetchCollection = async () => {
            try {
                setLoading(true);
                setError(null);

                const userId = user?.userId;
                if (!userId) {
                    setError("User not found");
                    return;
                }

                if (!collectionId) {
                    setError("Collection ID not found");
                    return;
                }

                // Get collections link from user
                const collectionsLink = user.links?.find(
                    link => link.rel === 'collections'
                );

                if (!collectionsLink) {
                    setError("Collections link not found");
                    return;
                }

                // Fetch all collections
                const collection = await getOneCollection(collectionsLink.href, collectionId);

                if (!collection) {
                    setError("Collection not found");
                    return;
                }

                setCollection(collection);

                if (collection.gamesId && collection.gamesId.length > 0) {
                    const gameDetails = await Promise.all(
                        collection.gamesId.map(async (gameId) => {
                            try {
                                const gameInfo = await getGameInfo(gameId, i18n.language);
                                return {
                                    id: gameInfo.id,
                                    name: gameInfo.name,
                                    image: gameInfo.image
                                };
                            } catch (err) {
                                console.error(`Error fetching game info for ${gameId}:`, err);
                                return {};
                            }
                        })
                    );
                    setGames(gameDetails);
                } else {
                    setGames([]);
                }
            } catch (err) {
                console.error("Error fetching collection:", err);
                setError(err.message || "Failed to fetch collection");
            } finally {
                setLoading(false);
            }
        };

        if (user?.userId) {
            fetchCollection();
        }
    }, [collectionId, user, i18n.language]); // eslint-disable-line react-hooks/exhaustive-deps

    useEffect(() => {
        const timeoutId = setTimeout(async () => {
            if (!searchTerm.trim()) {
                setSearchResults([]);
                setSearchLoading(false);
                return;
            }

            setSearchLoading(true);
            try {
                const results = await searchGames(searchTerm, i18n.language);
                setSearchResults(results);
            } catch (err) {
                console.error("Search error:", err);
                setSearchResults([]);
            } finally {
                setSearchLoading(false);
            }
        }, 400);

        return () => clearTimeout(timeoutId);
    }, [searchTerm, i18n.language]); // eslint-disable-line react-hooks/exhaustive-deps

    // Gérer l'ajout d'un jeu à la collection
    const handleAddGame = async (game) => {
        try {
            const addGameCollectionLink = collection.links?.find(
                link => link.rel === 'add-game'
            );

            if (!addGameCollectionLink) {
                console.error("Add game link not found");
                return;
            }

            await addGameCollection(addGameCollectionLink.href, game.id);
            setGames(prevGames => [...prevGames, game]);
            setCollection({
                ...collection,
                gamesId: [...collection.gamesId, game.id]
            });

            // Supprimer le jeu des résultats de recherche
            setSearchResults(prevResults =>
                prevResults.filter(result => result.id !== game.id)
            );
        } catch (err) {
            console.error("Error adding game to collection:", err);
        }
    };

    // Gérer la suppression d'un jeu de la collection
    const handleRemoveGame = async (game) => {
        try {
            // Get the remove-game link from collection
            const removeGameCollectionLink = collection.links?.find(
                link => link.rel === 'remove-game'
            );

            if (!removeGameCollectionLink) {
                console.error("Remove game link not found");
                return;
            }

            // Appelez l'API pour supprimer le jeu
            await removeGameCollection(removeGameCollectionLink.href, game.id);

            setGames(prevGames => prevGames.filter(g => g.id !== game.id));
            setCollection({
                ...collection,
                gamesId: collection.gamesId.filter(id => id !== game.id)
            });


            // Réafficher le jeu dans les résultats de recherche
            setSearchResults(prevResults => [...prevResults, game]);
        } catch (err) {
            console.error("Error removing game from collection:", err);
        }
    };

    const GameRow = ({ game, showRemoveButton = false }) => (
        <Link to={`/Game/${game.id}`} className="button-no-style" target="_blank">
            <div className="game-row">
                <div className="gthumb ">
                    {imageErrors[game.id] ? (
                        <i className="ti ti-device-gamepad-2"></i>
                    ) : (
                        <img
                            src={game.image}
                            alt={game.name}
                            onError={() => handleImageError(game.id)}

                        />
                    )}

                </div>
                <div className="ginfo">
                    <div className="gname">{game.name}</div>
                    <div className="gmeta">App ID: {game.id}</div>
                </div>
                <div className="gactions">
                    {showRemoveButton ? (
                        <button className="btn btn-delete" onClick={(event) => {event.preventDefault(); handleRemoveGame(game);}}>
                            <i className="ti ti-trash"></i>
                        </button>
                    ) : (
                        <button className="btn btn-primary" onClick={(event) => {event.preventDefault(); handleAddGame(game);}}>
                            <i className="ti ti-plus"></i>
                        </button>
                    )
                    }
                </div>
            </div>
        </Link>

    );

    if (loading) {
        return (
            <main className="collection-page">
                <div className="message">{t('Dashboard.loading')}</div>
            </main>
        );
    }

    if (error) {
        return (
            <main className="collection-page">
                <div className="message error">{error}</div>
            </main>
        );
    }

    const handleDeleteCollection = async () => {
        if (!window.confirm(t('Collection.ConfirmDeleteCollection'))) {
            return;
        }

        try {
            // Récupérer le lien de suppression de la collection
            const deleteCollectionLink = collection?.links?.find(
                link => link.rel === 'delete-collection'
            );

            if (!deleteCollectionLink) {
                console.error("Delete collection link not found");
                return;
            }

            await deleteCollection(deleteCollectionLink.href);
            navigate('/dashboard');
        } catch (err) {
            console.error("Error deleting collection:", err);
        }
    };

    const handleUpdateCollectionLabel = async (label) => {
        const updateLink = collection?.links?.find(link => link.rel === 'update-collection');
        if (!updateLink) {
            console.error("Update collection link not found");
            return;
        }

        await updateCollection(updateLink.href, { ...collection, label });
        setCollection(prev => ({ ...prev, label: label }));
        setShowEditModal(false);
    };


    return (
        <main className="collection-page">
            <div className="collection-header">
                <div className="header-actions">
                    <button
                        className="btn btn-edit"
                        onClick={() => {
                            setShowEditModal(true);
                        }}
                    >
                        <i className="ti ti-edit"></i>
                    </button>
                    <button
                        className="btn btn-danger"
                        onClick={handleDeleteCollection}
                    >
                        {t('Collection.DeleteCollection')}
                    </button>
                </div>
                <h1>{collection?.label || t('Collection.Untitled')}</h1>
                <div className="collection-meta">
                    {games.length} {t('Collection.GamesCount')}
                </div>
            </div>

            <div className="collection-content">
                <div className="section glass">
                    <div className="sec-head">
                        <span className="sec-title">{t('Collection.Games')}</span>
                    </div>

                    {games.length > 0 ? (
                        <div className="games-list">
                            {games.map(game => (
                                <GameRow
                                    key={game.id}
                                    game={game}
                                    showRemoveButton={true}
                                />
                            ))}
                        </div>
                    ) : (
                        <div className="message">{t('Collection.NoGames')}</div>
                    )}
                </div>

                <div className="section glass">
                    <div className="sec-head">
                        <span className="sec-title">{t('Collection.AddGame')}</span>
                    </div>

                    <div className="input-wrap input-collection-search">
                        <i className="ti ti-search" aria-hidden="true"></i>

                        <input
                            type="text"
                            placeholder={t('Collection.SearchPlaceholder')}
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>

                    {searchTerm.length > 0 && (
                        <div className="search-results">
                            <h3>{t('Collection.SearchResults')}</h3>

                            {/* État de chargement */}
                            {searchLoading ? (
                                <div className="search-loading">
                                    <div className="spinner-small"></div>
                                    <span>{t('Dashboard.loading')}</span>
                                </div>
                            ) : searchResults.length < 1 ? (
                                <span className="no-results">
                                    {t('Collection.NoSearchResults')}
                                </span>
                            ) : (
                                searchResults
                                    // si un jeux est déjà dans la collection, alors il ne s'affiche pas dans les resultats
                                    .filter(game => !games.some(g => g.id === game.id))
                                    .map(game => (
                                        <GameRow key={game.id} game={game} />
                                    ))
                            )}
                        </div>
                    )}
                </div>
            </div>
            <CollectionModal
                isOpen={showEditModal}
                onClose={() => setShowEditModal(false)}
                onSubmit={handleUpdateCollectionLabel}
                t={t}
                initialLabel={collection.label}
                isEditing={true}
            />
        </main>
    );
}