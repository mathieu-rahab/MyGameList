import GamePreview from '../../components/GamePreview';
import { Link } from 'react-router';

const placeholder = { /*TODO: récup les info de jeux de l'API */ 
    title: "UnSuperJeu",
    coverUrl: "/covers/placeholder.png",
    category: "FriendSlop",
    version: "V 0.Trust WIP",
    year: 2077,
    size: "420.69 MB"
};


const gamesData = Array.from({ length: 24 }, (_, i) => ({
    ...placeholder,
    id: i + 1 
}));

export default function Home(){
    return (
        <div>
            <h1>MyGameList</h1>

            <div className="library-container" style={{ '--columns': 6 }}>
                {gamesData.map((game) => (
                    <Link to={"/Game"}>
                        <button>
                            <GamePreview key={game.id} game={game} />
                        </button>
                    </Link>
                    
                ))}
            </div>
        </div>
        
    )
}