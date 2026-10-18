using VenteJour.Models;

namespace VenteJour.Services.Fake;

/// <summary>Contenu d'aide embarqué (démo). Le backend pourra fournir ce même contenu via IAideService.</summary>
internal static class FakeAideContenu
{
    public static readonly List<CategorieAide> Categories = new()
    {
        new() { Id = "produits", Titre = "Gérer mes produits", Description = "Ajouter, modifier et suivre le stock", Icone = "box" },
        new() { Id = "benefices", Titre = "Comprendre mes bénéfices", Description = "Recettes, coûts et marges", Icone = "chart" },
        new() { Id = "ventes", Titre = "Gérer mes ventes", Description = "Enregistrer, consulter ou annuler une vente", Icone = "cart" },
        new() { Id = "compte", Titre = "Mon compte et mes données", Description = "Profil, kiosque et paramètres", Icone = "person_fill" }
    };

    private static ArticleAide A(string id, string cat, string titre, string resume, string[] paragraphes, params string[] motsCles)
    {
        var categorie = Categories.First(c => c.Id == cat);
        return new ArticleAide
        {
            Id = id, CategorieId = cat, CategorieTitre = categorie.Titre, Titre = titre, Resume = resume,
            Paragraphes = paragraphes.ToList(), MotsCles = motsCles.ToList()
        };
    }

    public static readonly List<ArticleAide> Articles = new()
    {
        A("ajouter-produit", "produits", "Ajouter un produit",
            "Créer un produit avec ses prix et son stock.",
            new[]
            {
                "1. Ouvrez l'onglet Produits, puis appuyez sur Ajouter.",
                "2. Saisissez le nom du produit et choisissez sa catégorie.",
                "3. Renseignez le prix d'achat (ce que vous payez à votre fournisseur), le prix de vente (ce que paie le client) et la quantité en stock.",
                "4. Appuyez sur Enregistrer. Le produit apparaît dans la liste et devient disponible dans Nouvelle vente."
            }, "nouveau", "créer", "catalogue"),

        A("modifier-produit", "produits", "Modifier un produit ou ajuster un prix",
            "Corriger un nom, un prix ou un stock.",
            new[]
            {
                "Dans l'onglet Produits, appuyez sur le produit à modifier, changez les valeurs puis appuyez sur Enregistrer.",
                "Un changement de prix ne s'applique qu'aux ventes futures. Les ventes déjà enregistrées gardent les prix du jour de la vente : leurs recettes et leurs bénéfices ne changent pas."
            }, "prix", "modification", "corriger"),

        A("gerer-stock", "produits", "Gérer les stocks",
            "Comprendre comment le stock évolue.",
            new[]
            {
                "Le stock diminue automatiquement à chaque vente enregistrée et il augmente lorsque vous annulez une vente.",
                "Il n'est pas possible de vendre plus d'articles que le stock disponible.",
                "Après un réapprovisionnement, ouvrez le produit et mettez à jour la quantité en stock."
            }, "quantité", "inventaire", "réapprovisionnement"),

        A("calcul-recette", "benefices", "Comment la recette est-elle calculée ?",
            "Recette d'une vente, d'un produit et de la journée.",
            new[]
            {
                "La recette d'une vente est égale à la quantité vendue multipliée par le prix de vente.",
                "La recette du jour est la somme des ventes valides de la journée. Les ventes annulées ne sont pas comptées.",
                "La recette n'est pas le bénéfice : elle ne tient pas compte de ce que vous avez payé pour acheter les produits."
            }, "chiffre d'affaires", "total", "encaissé"),

        A("calcul-benefice", "benefices", "Comment le bénéfice et la marge sont-ils calculés ?",
            "Bénéfice = recette - coût d'achat.",
            new[]
            {
                "Le coût d'achat d'une vente est la quantité multipliée par le prix d'achat appliqué le jour de la vente. Le bénéfice est la recette moins ce coût.",
                "La marge est le bénéfice divisé par la recette, exprimé en pourcentage.",
                "Exemple : 2 bouteilles vendues 800 FCFA et achetées 500 FCFA donnent une recette de 1 600 FCFA, un coût de 1 000 FCFA, un bénéfice de 600 FCFA et une marge de 37,5 %."
            }, "marge", "gain", "profit"),

        A("vente-perte", "benefices", "Que se passe-t-il en cas de vente à perte ?",
            "Prix de vente inférieur au prix d'achat.",
            new[]
            {
                "Si le prix de vente d'un produit est inférieur à son prix d'achat, l'application affiche un écran d'alerte avant d'enregistrer la vente.",
                "Vous pouvez ajuster le prix du produit, ou cocher la confirmation pour enregistrer la vente malgré la perte.",
                "Une vente à perte a un bénéfice négatif : elle réduit le bénéfice cumulé du produit et celui de la journée."
            }, "perte", "alerte", "négatif"),

        A("lire-classement", "benefices", "Lire le classement des produits",
            "Produits qui rapportent le plus.",
            new[]
            {
                "Le classement est accessible depuis l'accueil, avec Voir tout dans la section Top 3 produits.",
                "Deux classements sont disponibles : par recettes et par bénéfices. Vous pouvez choisir Aujourd'hui, Cette semaine, Ce mois ou une période personnalisée.",
                "Le classement se recalcule à partir des ventes valides : une nouvelle vente le met à jour, une vente annulée en est retirée.",
                "En cas d'égalité, les produits sont départagés par les autres résultats, puis par la quantité vendue et enfin par ordre alphabétique. Les produits sans vente sur la période n'apparaissent pas."
            }, "top", "meilleurs produits", "rang", "classer"),

        A("enregistrer-vente", "ventes", "Enregistrer une vente",
            "Du bouton + à l'écran de confirmation.",
            new[]
            {
                "1. Appuyez sur le bouton orange + de la barre du bas.",
                "2. Choisissez l'article et la quantité. Vérifiez les prix et le total.",
                "3. Appuyez sur Valider. Un écran confirme le montant encaissé et le bénéfice.",
                "Vous pouvez ensuite enregistrer une autre vente ou revenir à l'accueil."
            }, "nouvelle vente", "valider", "encaisser"),

        A("historique", "ventes", "Consulter l'historique",
            "Retrouver les ventes passées.",
            new[]
            {
                "Ouvrez l'onglet Historique et choisissez Aujourd'hui, Hier ou Semaine.",
                "Appuyez sur une vente pour voir son détail : produits, montant, coût d'achat, marge et mode de règlement.",
                "Les ventes annulées restent visibles dans la liste mais ne sont pas incluses dans les totaux."
            }, "transactions", "détail", "liste"),

        A("annuler-vente", "ventes", "Annuler une vente",
            "Corriger une vente enregistrée par erreur.",
            new[]
            {
                "Ouvrez la vente depuis l'Historique, puis appuyez sur Annuler la vente et confirmez.",
                "Les articles sont remis en stock et la vente est retirée des recettes, des bénéfices et des classements.",
                "L'annulation est définitive. Pour corriger une quantité, annulez la vente puis enregistrez-la de nouveau."
            }, "supprimer", "erreur", "corriger"),

        A("infos-kiosque", "compte", "Modifier les informations du kiosque",
            "Nom, responsable, téléphone et adresse.",
            new[]
            {
                "Allez dans Profil, puis Informations du kiosque.",
                "Modifiez les champs voulus. Choisissez d'abord la région, puis la ville, puis le quartier : chaque liste dépend du choix précédent.",
                "Le téléphone doit être un numéro mobile camerounais à 9 chiffres commençant par 6.",
                "Appuyez sur Enregistrer les modifications. Si vous quittez la page avec des changements non enregistrés, l'application vous demande de confirmer."
            }, "adresse", "téléphone", "région", "ville", "quartier"),

        A("profil-parametres", "compte", "Profil et paramètres",
            "Notifications, thème et langue.",
            new[]
            {
                "Dans Profil, ouvrez Paramètres pour activer ou couper les alertes de vente à perte et les rappels de clôture, et pour choisir le thème clair ou sombre.",
                "La langue choisie est mémorisée. L'interface est actuellement disponible en français."
            }, "thème", "sombre", "notifications", "langue"),

        A("donnees", "compte", "Mes données et leur sauvegarde",
            "Où sont conservées vos informations.",
            new[]
            {
                "Dans cette version de démonstration, les données sont conservées en mémoire sur votre téléphone et ne sont pas envoyées à un serveur. Elles sont réinitialisées lorsque l'application est fermée.",
                "La synchronisation avec un serveur sera ajoutée dans une version ultérieure."
            }, "sauvegarde", "serveur", "confidentialité", "sécurité")
    };
}
