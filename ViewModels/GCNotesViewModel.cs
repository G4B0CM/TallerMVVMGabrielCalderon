using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TallerMVVMGabrielCalderon.Models;

namespace TallerMVVMGabrielCalderon.ViewModels
{
    internal class GCNotesViewModel : IQueryAttributable
    {
        public ObservableCollection<ViewModels.GCNoteViewModel> AllNotes { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectNoteCommand { get; }

        public GCNotesViewModel()
        {
            AllNotes = new ObservableCollection<ViewModels.GCNoteViewModel>(Models.Note.LoadAll().Select(n => new GCNoteViewModel(n)));
            NewCommand = new AsyncRelayCommand(NewNoteAsync);
            SelectNoteCommand = new AsyncRelayCommand<ViewModels.GCNoteViewModel>(SelectNoteAsync);
        }

        private async Task NewNoteAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.NotePage));
        }

        private async Task SelectNoteAsync(ViewModels.GCNoteViewModel note)
        {
            if (note != null)
                await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}?load={note.Identifier}");
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("deleted"))
            {
                string noteId = query["deleted"].ToString();
                GCNoteViewModel matchedNote = AllNotes.Where((n) => n.Identifier == noteId).FirstOrDefault();

                // If note exists, delete it
                if (matchedNote != null)
                    AllNotes.Remove(matchedNote);
            }
            else if (query.ContainsKey("saved"))
            {
                string noteId = query["saved"].ToString();
                GCNoteViewModel matchedNote = AllNotes.Where((n) => n.Identifier == noteId).FirstOrDefault();

                // If note is found, update it
                if (matchedNote != null)
                    matchedNote.Reload();

                // If note isn't found, it's new; add it.
                else
                    AllNotes.Add(new GCNoteViewModel(Note.Load(noteId)));
            }
        }
    }
}
