using System.Collections;
using System.Collections.Specialized;

namespace VenteJour.Views.Controls;

/// <summary>
/// Menu déroulant commun à toute l'application (articles, catégories, régions, villes, quartiers, langue...).
/// Toute la zone du champ ouvre le menu : flèche, texte, icône et marges.
/// </summary>
public partial class SelectField : ContentView
{
    private bool _synchro;
    private INotifyCollectionChanged _collection;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable), typeof(SelectField), null,
        propertyChanged: (b, o, n) => ((SelectField)b).ApresChangementSource((IEnumerable)o, (IEnumerable)n));

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(SelectField), null, BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((SelectField)b).ApresChangementSelection());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(SelectField), "Choisir",
        propertyChanged: (b, o, n) => ((SelectField)b).Rafraichir());

    public static readonly BindableProperty IconeProperty = BindableProperty.Create(
        nameof(Icone), typeof(string), typeof(SelectField), null,
        propertyChanged: (b, o, n) =>
        {
            var champ = (SelectField)b;
            var source = n as string;
            champ.IconeImage.Source = string.IsNullOrEmpty(source) ? null : source;
            champ.Tuile.IsVisible = !string.IsNullOrEmpty(source);
        });

    /// <summary>Nom de la propriété affichée pour des objets (ex. "Nom"). Vide pour des chaînes.</summary>
    public static readonly BindableProperty CheminAffichageProperty = BindableProperty.Create(
        nameof(CheminAffichage), typeof(string), typeof(SelectField), string.Empty,
        propertyChanged: (b, o, n) =>
        {
            var champ = (SelectField)b;
            var chemin = n as string;
            champ.Selecteur.ItemDisplayBinding = string.IsNullOrEmpty(chemin) ? null : new Binding(chemin);
            champ.Rafraichir();
        });

    public static readonly BindableProperty TitreMenuProperty = BindableProperty.Create(
        nameof(TitreMenu), typeof(string), typeof(SelectField), string.Empty,
        propertyChanged: (b, o, n) => ((SelectField)b).Selecteur.Title = n as string);

    public SelectField()
    {
        InitializeComponent();
        Tuile.IsVisible = false;
        Rafraichir();
    }

    public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public string Icone { get => (string)GetValue(IconeProperty); set => SetValue(IconeProperty, value); }
    public string CheminAffichage { get => (string)GetValue(CheminAffichageProperty); set => SetValue(CheminAffichageProperty, value); }
    public string TitreMenu { get => (string)GetValue(TitreMenuProperty); set => SetValue(TitreMenuProperty, value); }

    private void ApresChangementSource(IEnumerable ancienne, IEnumerable nouvelle)
    {
        if (_collection != null)
            _collection.CollectionChanged -= OnCollectionChanged;
        _collection = nouvelle as INotifyCollectionChanged;
        if (_collection != null)
            _collection.CollectionChanged += OnCollectionChanged;

        _synchro = true;
        try
        {
            Selecteur.ItemsSource = nouvelle as IList ?? nouvelle?.Cast<object>().ToList();
            if (SelectedItem != null)
                Selecteur.SelectedItem = SelectedItem;
        }
        finally
        {
            _synchro = false;
        }
        Rafraichir();
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => Rafraichir();

    private void ApresChangementSelection()
    {
        _synchro = true;
        try
        {
            if (!Equals(Selecteur.SelectedItem, SelectedItem))
                Selecteur.SelectedItem = SelectedItem;
        }
        finally
        {
            _synchro = false;
        }
        Rafraichir();
    }

    private void OnSelecteurChange(object sender, EventArgs e)
    {
        if (_synchro)
            return;

        // Sélection faite par l'utilisateur (ou remise à zéro par le Picker) : on la propage au ViewModel.
        SelectedItem = Selecteur.SelectedItem;
        Rafraichir();
    }

    private string Libelle(object element)
    {
        if (element == null) return null;
        if (string.IsNullOrEmpty(CheminAffichage)) return element.ToString();
        var propriete = element.GetType().GetProperty(CheminAffichage);
        return propriete?.GetValue(element)?.ToString() ?? element.ToString();
    }

    private void Rafraichir()
    {
        var libelle = Libelle(SelectedItem);
        var aSelection = !string.IsNullOrEmpty(libelle);
        ValeurLabel.Text = libelle;
        ValeurLabel.IsVisible = aSelection;
        PlaceholderLabel.Text = Placeholder;
        PlaceholderLabel.IsVisible = !aSelection;

        // Un menu sans option (ex. ville avant d'avoir choisi la région) est désactivé et atténué.
        var actif = ItemsSource != null && ItemsSource.Cast<object>().Any();
        Selecteur.IsEnabled = actif;
        Cadre.Opacity = actif ? 1 : 0.55;
    }
}
