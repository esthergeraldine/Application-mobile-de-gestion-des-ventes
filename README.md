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
