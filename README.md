# My Game List
Projet universitaire
## Equipe

- Boussad, Mohand Said Ramy, mohand-said-ram.boussad@etu.univ-littoral.fr
- Duperrin, Noe, noe.duperrin@etu.univ-littoral.fr
- Rahab, Mathieu, mathieu.rahab@etu.univ-littoral.fr

## Présentation du projet

À l'image de My Anime List pour les animes, My Game List permet de repertorier et ranger ses jeux dans des collections (limité aux jeux Steam à l'heure d'écriture)

## Technologies utilisées
### Backend .Net 10:
Le backend est développé avec **ASP.NET Core MVC** et expose une API REST consommée par le frontend React.

Fonctionnalités principales :

* Gestion des utilisateurs
* Authentification par token JWT
* Gestion des collections de jeux
* Ajout et suppression de jeux dans les collections
* Gestion des relations d’amitié
* Recherche de jeux via API [Steam](<https://steamcommunity.com/dev>)
* Récupération d’informations Steam : jeux, progression, succès récents
* Documentation API avec Swagger
* Gestion CORS pour autoriser frontend local
* Gestion centralisée des erreurs avec middleware
* Ajout de liens HATEOAS dans certaines réponses API

Base de données :

* SQLite
* Entity Framework Core
* Migrations appliquées automatiquement au démarrage de API

### Frontend Vite, React:
* Traductions [i18n](<https://www.i18next.com/>)
* Test [Cypress](<https://www.cypress.io/>)


## Gestion de projet

### Développement:
* Backend sur [JetBrains Rider](<https://www.jetbrains.com/rider/>).
* Frontend sur [VSCode](<https://code.visualstudio.com/download>)/[Codium](<https://vscodium.com/>).
### Communication/Collaboration:
En présenciel, via l'interface Gitlab (Issue Board) et sur [Discord](<https://discord.com/>).

## Expérience générale

Pas d'experience antérieure en .NET, C#, React et le dévlopement de WebApp en général. Ce projet nous a fait découvrir plusieurs outils qu'on utilisera probablement dans le futur. 

## Installation

### Prérequis :

* .NET SDK 10
* ASPNET Runtime 10
* Rider, Visual Studio ou VS Code avec extension C#
* Une [clé API](<https://steamcommunity.com/dev/apikey>) Steam
* SQLite ne nécessite pas forcément installation séparée si utilisé via Entity Framework Core
* Node.js

Cloner le projet :
```bash
git clone https://github.com/mathieu-rahab/MyGameList.git
```
#### Variables d’environnement backend

Le backend utilise un fichier `.env` situé dans `/backend/Mygamelist/Mygamelist.API/`. un fichier `TEST.env` est à renommer et completer.

Variables nécessaires : ```env STEAM_KEY=cle_api_steam AUTH_KEY=cle_secrete_jwt```

#### Installation backend

```
cd backend/Mygamelist/Mygamelist.API/
dotnet restore
```
<!-- sudo dotnet workload update -->

#### Installation Frontend

```
cd frontend/
npm install
```

## Utilisation

### Frontend
Depuis `/frontend/`, lancer le front via `npm run dev`

### Backend
Depuis `/backend/Mygamelist/Mygamelist.API/`, lancer le back via `dotnet run Mygamelist.API`

### Note:
un compte admin doit être créé à l'installation du projet avec l'email `root@root.com`
#### example:
dans le swagger renseigner le body de la requête `POST /api/USER` avec:
```
{
  "pseudo": "groot",
  "email": "root@root.com",
  "password": "Ro0t123"
}
```

#### Routes d'API
url du swagger : fixé à [localhost:5131/swagger](<http://localhost:5131/swagger>)


##### Identity

```
[POST] /api/identity/token -> génère un token JWT à partir de email + mot de passe 
[POST] /api/identity/renew -> renouvelle token JWT utilisateur connecté
``` 

##### User
```
[GET] /api/user -> retourne tous les utilisateurs, réservé admin 
[GET] /api/user/{id} -> retourne utilisateur {id} 
[POST] /api/user -> crée un utilisateur 
[PUT] /api/user/{id} -> remplace les informations utilisateur {id}
[PATCH] /api/user/{id} -> modifie partiellement utilisateur {id}
[DELETE] /api/user/{id} -> supprime utilisateur {id}
[GET] /api/user/{id}/games -> retourne les jeux Steam de l'utilisateur {id}
[GET] /api/user/{id}/recent-games -> retourne jeux récemment joués de l'utilisateur {id}
[GET] /api/user/{id}/progression-game/{appId} -> retourne progression utilisateur {id} sur jeu Steam {appId}
[GET] /api/user/{id}/recent-achievements -> retourne succès récents utilisateur {id}
``` 

##### Collection
```
[GET] /api/user/{userId}/collection -> retourne collections utilisateur {userId}
[GET] /api/user/{userId}/collection/{id} -> retourne collection {id} de l'utilisateur {userId}
[POST] /api/user/{userId}/collection -> crée collection vide pour l'utilisateur {userId}
[PUT] /api/user/{userId}/collection/{id} -> modifie collection {id} de l'utilisateur {userId}
[DELETE] /api/user/{userId}/collection/{id} -> supprime collection {id} de l'utilisateur {userId}
[POST] /api/user/{userId}/collection/{id}/game -> ajoute jeu à collection {id} de l'utilisateur {userId}
[DELETE] /api/user/{userId}/collection/{id}/game -> retire jeu de collection {id} de l'utilisateur {userId}
``` 

##### Friendship
```
[GET] /api/friendship -> retourne toutes les relations, réservé admin 
[GET] /api/friendship/{id} -> retourne relation d’amitié {id} 
[POST] /api/friendship -> crée demande d’amitié 
[PUT] /api/friendship/{id} -> modifie statut demande d’amitié 
[DELETE] /api/friendship/{id} -> supprime relation d’amitié 
[GET] /api/friendship/pending/sent -> retourne demandes envoyées en attente 
[GET] /api/friendship/pending/received -> retourne demandes reçues en attente 
[GET] /api/friendship/friends -> retourne liste amis utilisateur
``` 

##### Steam
```
[GET] /api/steam/game/{gameId}?l={language} -> retourne informations jeu Steam 
[GET] /api/steam/search?term={term}&l={language}&cc={countryCode} -> recherche jeux Steam
```

Note sécurité à ajouter :
##### Authentification

Certaines routes nécessitent token JWT.

Après connexion via : ```POST /api/identity/token``` 

Ajouter token dans header HTTP : ```Authorization: Bearer``` 

Swagger permet aussi de tester routes protégées via bouton **Authorize**.
