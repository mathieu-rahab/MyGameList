import GameDetail from '../../components/GameDetail';
import './index.css';

const placeholder = { /*TODO: récup les info de jeux de l'API */ 
    title: "UnSuperJeu",
    coverUrl: "/covers/placeholder.png",
    bgUrl: "/covers/placeholderBackground.jpg",
    category: "FriendSlop",
    version: "V 0.Trust WIP",
    year: 2077,
    size: "420.69 MB",
    description: "The DarkSouls of its genra, as no journalist has completed the tutorial yet.\nThe GOTY of this year. Better than everything Riot Games did since the last decade.\nSurprisingly, Capcom did not release it, yet they wish they did."
};


export default function Game(){
    return (
        <div className='main'>
            
            <h1>MyGameList</h1>
            <GameDetail game={placeholder} className="GameDetail"/>
        </div>
        
    )
}