using Avalonia.Controls.Mixins;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetStalkerAvalonia.Core.ViewModels;
using ReactiveUI;
using System;

namespace NetStalkerAvalonia.Core.Views;

public partial class StatusMessageView : ReactiveWindow<StatusMessageViewModel>
{
	public StatusMessageView()
	{
		this.WhenActivated(disposables => { ViewModel!.Close.Subscribe(x => Close()).DisposeWith(disposables); });

		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}