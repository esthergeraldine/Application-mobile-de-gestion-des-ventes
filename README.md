# VenteJour — .NET MAUI (Android)

## Ouvrir et lancer
1. Visual Studio Community avec la charge de travail **« Développement d'applications mobiles .NET (MAUI) »**.
2. Ouvrir `VenteJour.sln`.
3. Choisir l'émulateur **Android Pixel**, puis F5.

Le `.csproj` cible `net8.0-android`. Si votre workload est en .NET 9 ou 10, changez uniquement `TargetFrameworks`
(ex. `net9.0-android`).

## Architecture
- `Models/` : Produit, Vente, LigneVente, Utilisateur, Bilan (+ ClassementProduit, PeriodeVente).
- `Services/Interfaces/` : IProduitService, IVenteService, IBilanService, IAuthService, IParametresService.
- `Services/Fake/` : implémentations fictives + `FakeDataStore` (seul endroit contenant les données de démo).
- `Services/Api/` : squelette `HttpProduitService` pour la future API ASP.NET Core.
- `ViewModels/` : MVVM (CommunityToolkit.Mvvm), aucune dépendance à une base de données.
- `Views/` : pages XAML (sans donnée en dur) + contrôles `BottomNav`, `AppHeader`, `BackHeader`, `VenteFormFields`.

## Brancher la future API
Dans `MauiProgram.cs`, remplacer les 4 lignes `AddSingleton<I...Service, Fake...Service>()` par des services HTTP
(voir le commentaire `AddHttpClient`). Aucun écran ni ViewModel à modifier.

## Données de démo
Aujourd'hui : 12 transactions, 27 articles, 15 500 FCFA, bénéfice 5 200 FCFA (marge 33,5 %), +30 % vs hier.
Connexion : n'importe quel identifiant + mot de passe non vides.
Pour tester l'écran « Aucun produit » : `FakeDataStore.InclureDonneesDemo = false`.
Pour tester la vente à perte : choisir « Jus d'orange 1L » (achat 900, vente 800).

---
## Mise à jour : kiosque, aide et support, classement

- **SelectField** (`Views/Controls/SelectField.*`) : menu déroulant unique de l'application ; un Picker invisible couvre
  toute la zone (icône, texte, flèche) donc n'importe quel appui ouvre le menu.
- **Informations du kiosque** : région → ville → quartier en cascade, validation, indicateur de chargement,
  confirmation avant de quitter avec des modifications non enregistrées.
- **Aide et support** : recherche (sans accents ni casse), 4 catégories, 13 articles, formulaire de contact,
  signalement de problème (pièce jointe facultative).
- **Classement** (Accueil > « Voir tout ») : par recettes ou bénéfices, Aujourd'hui / Cette semaine / Ce mois / Personnalisée.
  Recalculé à chaque affichage à partir des ventes valides. Les ventes annulées (Historique > détail > Annuler) sont exclues.
- Règles de calcul : `Domain/StatistiquesCalculator.cs` (sans dépendance MAUI, testable).
- Tests : `dotnet run --project ../VenteJour.Tests` (dossier voisin).

Tout ce qui est « simulation » est isolé dans `Services/Fake/` (données en mémoire, rien n'est envoyé à un serveur).
Pour brancher l'API : remplacer les enregistrements `Fake...Service` de `MauiProgram.cs` par des services HTTP.


---
## Comptes utilisateurs (version finale)

- **Inscription réelle** : e-mail ou téléphone + nom du kiosque + téléphone + mot de passe (6 caractères minimum, confirmation).
  Les doublons (e-mail ou téléphone) sont refusés.
- **Connexion réelle** : compte inconnu → « Aucun compte… » ; mauvais mot de passe → « Mot de passe incorrect ».
  Connexion possible par e-mail (majuscules acceptées) ou par numéro de téléphone.
- **Mot de passe protégé** : seul un hash PBKDF2-SHA256 (sel aléatoire, 100 000 itérations) est stocké, dans `comptes.json`.
- **« Se souvenir de moi »** : rouvre directement le compte au lancement suivant ; sinon retour à la connexion.
- **Données séparées par compte** : une base SQLite par compte (`ventejour-<id>.db3`). Un compte neuf démarre à ZÉRO :
  aucun produit, aucune vente, aucune donnée de démonstration.
- **Démonstration à la demande** : Paramètres > « Charger des données de démonstration » (captures d'écran, vidéo).
- Limite : les comptes sont enregistrés sur le téléphone, sans serveur (pas de partage entre appareils, pas de récupération
  de mot de passe). Le futur backend remplacera `AuthService` / `JsonCompteRepository` par des appels à l'API.

## Mot de passe oublié (sans serveur)
- À la création du compte, un **code de récupération** (ex. `K7M2-9QXA-R4TD`) est affiché UNE seule fois. Seule son empreinte est stockée.
- « Mot de passe oublié ? » : identifiant + code de récupération + nouveau mot de passe. Un **nouveau code** est alors remis (un code ne sert qu'une fois).
- Après 5 essais incorrects (connexion ou code), blocage d'une minute pour ce compte.
- Sans le code, il faut créer un nouveau compte : sans serveur (e-mail/SMS), il n'existe pas d'autre moyen sûr de récupérer un compte.
