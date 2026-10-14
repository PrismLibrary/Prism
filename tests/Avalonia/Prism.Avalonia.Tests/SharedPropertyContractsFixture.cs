using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Prism.Common;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Avalonia.Tests;

public class SharedPropertyContractsFixture
{
    [Fact]
    public void ObservableValuePreservesDefaultOwnerAndNotifications()
    {
        var first = new ObservableObject<string>();
        var second = new ObservableObject<string>();
        var changes = new List<(object?, string?)>();
        ((INotifyPropertyChanged)first).PropertyChanged += (sender, args) => changes.Add((sender, args.PropertyName));
        var secondChanges = 0;
        ((INotifyPropertyChanged)second).PropertyChanged += (_, _) => secondChanges++;

        Assert.Null(first.Value);
        Assert.IsType<StyledProperty<string>>(ObservableObject<string>.ValueProperty);
        Assert.Equal(typeof(Control), ObservableObject<string>.ValueProperty.OwnerType);
        first.Value = "first";
        first.Value = "first";
        first.Value = "second";

        Assert.Equal(new[] { ((object?)first, (string?)"Value"), ((object?)first, (string?)"Value") }, changes);
        Assert.Equal(0, secondChanges);
        Assert.Null(second.Value);
    }

    [Fact]
    public void ObservableValueTypeDefaultIsPreserved()
    {
        Assert.Equal(0, new ObservableObject<int>().Value);
    }

    [Fact]
    public void ItemMetadataOnlyNotifiesForActiveChangesAndExplicitInvocation()
    {
        var item = new object();
        var metadata = new ItemMetadata(item);
        var second = new ItemMetadata(new object());
        var senders = new List<object?>();
        metadata.MetadataChanged += (sender, _) => senders.Add(sender);
        var secondChanges = 0;
        second.MetadataChanged += (_, _) => secondChanges++;

        Assert.Same(item, metadata.Item);
        Assert.Null(metadata.Name);
        Assert.False(metadata.IsActive);
        Assert.IsType<StyledProperty<string>>(ItemMetadata.NameProperty);
        Assert.IsType<StyledProperty<bool>>(ItemMetadata.IsActiveProperty);
        metadata.Name = "view";
        Assert.Empty(senders);
        metadata.IsActive = true;
        metadata.IsActive = true;
        metadata.IsActive = false;
        metadata.InvokeMetadataChanged();

        Assert.Equal(new object?[] { metadata, metadata, metadata }, senders);
        Assert.Equal(0, secondChanges);
        Assert.False(second.IsActive);
    }
}
