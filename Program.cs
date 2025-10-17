using System;
namespace ContactBookF;

public class ContactBookF
{
    // List to store all contact entries
    List<Contact> contacts;
    // Number of contacts to display per page when listing
    int itemsPerPage;

    // Constructor initializes contacts list, sets items per page, and starts UI
    public ContactBookF()
    {
        contacts = new List<Contact>();
        itemsPerPage = 10;

        WelcomeScreen();
        MainMenu();
    }

    // Displays welcome message with program info and waits for user to continue
    public void WelcomeScreen()
    {
        string welcome = @"
     Welcome to Contact Book!

     Author: Iander O. Santiago Figureoa

     Version: 1.0 Final

     Last Revised: 2025-14-05 4:51 PM

     Description:
     This program allows you to keep records of your contacts.

     Press ENTER to continue.
";
        Console.WriteLine(welcome);
        PressEnterToContinue();
    }

    // Displays main menu options and handles user selection with a loop
    public void MainMenu()
    {
        string menu = @"
[1] Load contacts from a file. 
[2] Show all contacts.
[3] Add contact.
[4] Edit contact.
[5] Delete contact. 
[6] Merge duplicated contacts. 
[7] Store contacts to a file. 
[8] Exit the application.
";
        int option = 8;
        do
        {
            Console.Clear();
            Console.WriteLine(menu);
            option = GetOption("Select an option: ", 1, 8);

            if (option == 1)
            {
                LoadEntriesFromFile();
            }
            else if (option == 2)
            {
                ShowAllContacts();
            }
            else if (option == 3)
            {
                AddNewContact();
                ShowAllContacts();
            }
            else if (option == 4)
            {
                EditContact();
            }
            else if (option == 5)
            {
                DeleteContact();
            }
            else if (option == 6)
            {
                MergeDuplicatedContacts();
            }
            else if (option == 7)
            {
                SaveContact();
            }
            else
            {
                Exit();
            }

        } while (option != 8);
        PressEnterToContinue();
    }

    // Prompts user for filename and loads contacts from it
    public void LoadEntriesFromFile()
    {
        Console.Clear();
        Console.WriteLine("Loading From: ");
        Console.WriteLine("Write the file name or nothing to cancel.");
        Console.WriteLine("File name: ");
        string filename = Console.ReadLine();

        LoadEntriesFromFile(filename);
    }

    // Loads contacts from the specified file, adding valid entries to the list
    public void LoadEntriesFromFile(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Console.WriteLine("The operation was canceled.");
            PressEnterToContinue();
        }
        else if (!File.Exists(filename))
        {
            Console.WriteLine($"ERROR: File \"{filename}\" was not found!");
            PressEnterToContinue();
        }
        else
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    string firstName = sr.ReadLine();
                    string lastName = sr.ReadLine();
                    string phoneNumber = sr.ReadLine();
                    string email = sr.ReadLine();

                    // Skip incomplete entries to avoid errors
                    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                        string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    Contact e = new Contact
                    {
                        firstName = firstName,
                        lastName = lastName,
                        phoneNumber = phoneNumber,
                        email = email
                    };

                    contacts.Add(e);
                }

                Console.WriteLine("Contact book loaded successfully!");
                PressEnterToContinue();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: An error occurred while reading the file! {ex.Message}");
            }
            finally
            {
                if (sr != null) { sr.Dispose(); }
            }
        }
    }

    // Displays all contacts with sorting and pagination options
    public void ShowAllContacts()
    {
        Console.Clear();
        Console.WriteLine("Sort by [1] Name [2] Last Name [3] Phone Number [4] E-mail:");
        int sortOption = GetOption("Choose sort option: ", 1, 4);

        switch (sortOption)
        {
            case 1:
                contacts = contacts.OrderBy(e => e.firstName).ToList();
                break;
            case 2:
                contacts = contacts.OrderBy(e => e.lastName).ToList();
                break;
            case 3:
                contacts = contacts.OrderBy(e => e.phoneNumber).ToList();
                break;
            case 4:
                contacts = contacts.OrderBy(e => e.email).ToList();
                break;
        }

        int page = 1;
        int pageCount = Math.Max(1, (int)Math.Ceiling(contacts.Count / (double)itemsPerPage));

        do
        {
            Console.Clear();
            Console.WriteLine("### Show All Contacts ###");
            Console.WriteLine(" #   First Name        Last Name             Phone         Email");
            Console.WriteLine("--------------------------------------------------------------------------");

            int offset = (page - 1) * itemsPerPage;
            int limit = Math.Min(offset + itemsPerPage, contacts.Count);

            for (int i = offset; i < limit; i++)
            {
                Contact e = contacts[i];
                string index = i.ToString().PadLeft(3, '0');

                string name = string.IsNullOrWhiteSpace(e.firstName) ? "(no name)" :
                              e.firstName.Length > 18 ? e.firstName.Substring(0, 15) + "..." : e.firstName;

                string lastName = string.IsNullOrWhiteSpace(e.lastName) ? "(no last)" :
                                  e.lastName.Length > 18 ? e.lastName.Substring(0, 15) + "..." : e.lastName;

                string phone = string.IsNullOrWhiteSpace(e.phoneNumber)
                    ? "(no phone)"
                    : (e.phoneNumber.Length == 10
                        ? $"{e.phoneNumber.Substring(0, 3)}-{e.phoneNumber.Substring(3, 3)}-{e.phoneNumber.Substring(6)}"
                        : e.phoneNumber);

                string email = string.IsNullOrWhiteSpace(e.email)
                    ? "(no email)"
                    : (e.email.Length > 22 ? e.email.Substring(0, 19) + "..." : e.email);

                Console.WriteLine($"{index}  {name,-18} {lastName,-18} {phone,-13} {email}");
            }

            // Fill empty lines if the last page is not full
            for (int i = 0; i < itemsPerPage - (limit - offset); i++)
            {
                Console.WriteLine();
            }

            Console.WriteLine($"\nPage {page} of {pageCount}");
            Console.Write("Go to [0] Main Menu [-] Previous Page [+] Next Page or Page Number: ");
            string nav = Console.ReadLine().Trim();

            if (nav == "+")
                page = Math.Min(page + 1, pageCount);
            else if (nav == "-")
                page = Math.Max(page - 1, 1);
            else if (int.TryParse(nav, out int p) && p >= 0 && p <= pageCount)
                page = p;
            else
                page = 0;

        } while (page != 0);
    }

    // Prompts user to enter new contact details and validates input before adding
    public void AddNewContact()
    {
        Console.Clear();
        string name, lastName, phoneNumber, email;
        Console.WriteLine(@"### Add New Contact ###
");

        // Ensure at least one of first or last name is provided
        do
        {
            Console.WriteLine("Enter First Name: ");
            name = Console.ReadLine();
            Console.WriteLine("Enter Last Name: ");
            lastName = Console.ReadLine();

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("You can't have both name and last name in blank. Please try again. :)");
            }

        } while (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(lastName));

        // Validate phone number length or presence of email
        do
        {
            Console.WriteLine("Enter Phone Number (max 10 digits): ");
            phoneNumber = Console.ReadLine();
            Console.WriteLine("Write an email or nothing if you prefer to leave it blank: ");
            email = Console.ReadLine();

            bool phoneValid = !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length <= 10;
            bool emailValid = !string.IsNullOrEmpty(email);

            if ((!phoneValid && !emailValid) || (phoneNumber.Length > 10))
            {
                Console.WriteLine("You must provide a valid phone number (up to 10 digits) or an email. Please try again. :)");
            }

        } while ((string.IsNullOrEmpty(phoneNumber) && string.IsNullOrEmpty(email)) || phoneNumber.Length > 10);

        // Confirm addition
        string option = GetOption("Answer [Y] to apply changes or [N] to cancel: ", "Y", "N");
        if (option == "Y")
        {
            Contact e = new Contact
            {
                firstName = name,
                lastName = lastName,
                phoneNumber = phoneNumber,
                email = email
            };
            contacts.Add(e);

            Console.WriteLine("Operation was successful.");
        }
        else
        {
            Console.WriteLine("Operation was canceled.");
        }
    }

    // Starts the process of editing a contact either by index or by searching a field
    public void EditContact()
    {
        Console.Clear();
        int option = GetOption("Search by [1] Index or [2] Field or [0] Cancel: ", 0, 2);

        if (option == 0)
        {
            Console.WriteLine("Operation canceled.");
            return;
        }

        if (option == 1)
        {
            EditContactByIndex();
        }
        else if (option == 2)
        {
            EditContactByField();
        }
    }

    // Lists contacts with indices and prompts user to select one for editing by index
    public void EditContactByIndex()
    {
        Console.Clear();
        Console.WriteLine("### Edit Existing Contact ###");
        Console.WriteLine(" #   Name       Last Name     Phone         E-mail");
        for (int i = 0; i < contacts.Count; i++)
        {
            var e = contacts[i];
            Console.WriteLine($"{i.ToString("D3")} {e.firstName.PadRight(10)} {e.lastName.PadRight(12)} {e.phoneNumber.PadRight(13)} {e.email}");
        }
        Console.WriteLine($"Page 1 of 1");
        int index = GetOption("Select index of which contact to edit: ", 0, contacts.Count - 1);
        EditContact(index);
    }

    // Searches contacts by first or last name and allows user to select one to edit
    public void EditContactByField()
    {
        Console.WriteLine("Enter the first name or last name of the contact to search for:");
        string searchTerm = Console.ReadLine().ToLower();
        var foundContacts = contacts.Where(e => e.firstName.ToLower().Contains(searchTerm) || e.lastName.ToLower().Contains(searchTerm)).ToList();

        if (foundContacts.Count == 0)
        {
            Console.WriteLine("No contacts found matching that name.");
            return;
        }

        Console.WriteLine("Found the following contacts:");
        for (int i = 0; i < foundContacts.Count; i++)
        {
            var e = foundContacts[i];
            Console.WriteLine($"[{i}] {e.firstName} {e.lastName}");
        }

        int selectedContactIndex = GetOption("Select contact by [0, 1, ...]: ", 0, foundContacts.Count - 1);
        EditContact(contacts.IndexOf(foundContacts[selectedContactIndex]));
    }


    // Edita un contacto en la posición especificada del listado
    public void EditContact(int index)
    {
        // Obtiene el contacto seleccionado por índice
        Contact e = contacts[index];

        // Muestra la información actual del contacto
        string text = $"[{index.ToString().PadLeft(3, '0')}]\nFirst Name: {e.firstName}\nLast Name: {e.lastName}\nPhone Number: {e.phoneNumber}\nEmail: {e.email}";
        Console.WriteLine(text);

        // Opciones para seleccionar qué campo editar o cancelar
        Console.WriteLine("[0] First Name [1] Last Name [2] Phone Number [3] Email [4] Cancel");
        int option = GetOption("Which property do you wish to edit? ", 0, 4);

        // Dependiendo de la opción elegida, solicita el nuevo valor o cancela
        if (option == 0)
        {
            Console.WriteLine("Enter new first name or nothing to cancel: ");
            string firstName = Console.ReadLine();

            if (string.IsNullOrEmpty(firstName))
                Console.WriteLine("Edit was canceled");
            else
            {
                Console.WriteLine("Edit was successful");
                e.firstName = firstName;
            }
        }
        else if (option == 1)
        {
            Console.WriteLine("Enter new last name or nothing to cancel: ");
            string lastName = Console.ReadLine();

            if (string.IsNullOrEmpty(lastName))
                Console.WriteLine("Edit was canceled");
            else
            {
                Console.WriteLine("Edit was successful");
                e.lastName = lastName;
            }
        }
        else if (option == 2)
        {
            Console.WriteLine("Enter new phone number or nothing to cancel: ");
            string phoneNumber = Console.ReadLine();

            if (string.IsNullOrEmpty(phoneNumber))
                Console.WriteLine("Edit was canceled");
            else
            {
                Console.WriteLine("Edit was successful");
                e.phoneNumber = phoneNumber;
            }
        }
        else if (option == 3)
        {
            Console.WriteLine("Enter new email or nothing to cancel: ");
            string email = Console.ReadLine();

            if (string.IsNullOrEmpty(email))
                Console.WriteLine("Edit was canceled");
            else
            {
                Console.WriteLine("Edit was successful");
                e.email = email;
            }
        }
        else
        {
            // Opción 4: cancelar edición
            Console.WriteLine("Edit was canceled");
        }
    }

    // Inicia el proceso para eliminar un contacto mostrando la lista
    public void DeleteContact()
    {
        Console.Clear();

        // Si no hay contactos, informa y regresa al menú
        if (contacts.Count == 0)
        {
            Console.WriteLine("No contacts found. Press any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("### Delete Existing Contact ###\n");

        // Muestra todos los contactos con índice y datos principales
        for (int i = 0; i < contacts.Count; i++)
        {
            Contact c = contacts[i];
            Console.WriteLine($"{i.ToString("D3")} {c.firstName.PadRight(10)} {c.lastName.PadRight(12)} {c.phoneNumber.PadRight(13)} {c.email}");
        }

        int s = 0;
        int e = contacts.Count - 1;

        // Solicita el índice del contacto a eliminar o cancelar (-1)
        int index = GetOption($"Choose index of contact to delete [{s}-{e}] or [-1] to cancel: ", -1, e);

        if (index == -1)
        {
            Console.WriteLine("Operation cancelled. Returning to the menu...");
            Thread.Sleep(1000);
            return;
        }

        // Llama a la sobrecarga que elimina contacto por índice
        DeleteContact(index);
    }

    // Elimina un contacto específico por índice, con confirmación del usuario
    public void DeleteContact(int index)
    {
        Contact c = contacts[index];

        // Confirma si se desea eliminar el contacto
        string option = GetOption("Are you sure you want to remove this contact? [Y/N]: ", "Y", "N");

        if (option.ToUpper() == "Y")
        {
            contacts.RemoveAt(index);
            Console.WriteLine("Contact was removed successfully.");
        }
        else
        {
            Console.WriteLine("Operation was cancelled.");
        }

        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
    }

    // Guarda la lista actual de contactos en un archivo de texto
    public void SaveContact()
    {
        Console.Clear();
        Console.WriteLine("Enter the file name to save entries (or press Enter to cancel):");
        string filename = Console.ReadLine();

        // Si no se ingresa nombre de archivo, cancela la operación
        if (string.IsNullOrWhiteSpace(filename))
        {
            Console.WriteLine("Operation canceled.");
            return;
        }

        try
        {
            // Escribe cada campo de cada contacto en líneas separadas
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (Contact entry in contacts)
                {
                    sw.WriteLine(entry.firstName);
                    sw.WriteLine(entry.lastName);
                    sw.WriteLine(entry.phoneNumber);
                    sw.WriteLine(entry.email);
                }
            }

            Console.WriteLine($"Entries successfully saved to \"{filename}\".");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: Failed to save the file. {ex.Message}");
        }
    }

    // Fusión y eliminación de contactos duplicados detectados
    public void MergeDuplicatedContacts()
    {
        Console.Clear();
        Console.WriteLine("### Merge & Deduplicate Contacts ###\n");

        // Obtiene grupos de contactos duplicados
        var duplicateGroups = FindDuplicatedContacts();

        if (duplicateGroups == null || duplicateGroups.Count == 0)
        {
            Console.WriteLine("No duplicate contacts were found.");
            PressEnterToContinue();
            return;
        }

        Console.WriteLine($"Found {duplicateGroups.Count} group(s) of duplicate contacts.\n");

        bool changesMade = false;
        int groupNumber = 1;

        // Procesa cada grupo de duplicados
        foreach (var group in duplicateGroups)
        {
            Console.WriteLine($"--- Duplicate Group #{groupNumber} ---");

            // Muestra los contactos dentro del grupo
            for (int i = 0; i < group.Count; i++)
            {
                var c = group[i];
                Console.WriteLine($"[{i}] {c.firstName} {c.lastName}, {c.phoneNumber}, {c.email}");
            }

            // Solicita al usuario elegir qué campo conservar para el contacto fusionado
            string mergedFirstName = AskUserToPickField("First Name", group.Select(e => e.firstName).ToList());
            string mergedLastName = AskUserToPickField("Last Name", group.Select(e => e.lastName).ToList());
            string mergedPhone = AskUserToPickField("Phone Number", group.Select(e => e.phoneNumber).ToList());
            string mergedEmail = AskUserToPickField("Email", group.Select(e => e.email).ToList());

            // Crea nuevo contacto fusionado con los campos elegidos
            Contact mergedEntry = new Contact
            {
                firstName = mergedFirstName,
                lastName = mergedLastName,
                phoneNumber = mergedPhone,
                email = mergedEmail
            };

            Console.WriteLine("\nNew merged contact preview:");
            Console.WriteLine($"{mergedEntry.firstName} {mergedEntry.lastName}, {mergedEntry.phoneNumber}, {mergedEntry.email}");

            // Confirma si desea agregar el contacto fusionado
            Console.Write("Add this merged contact? [y/n]: ");
            string confirmMerge = Console.ReadLine().Trim().ToLower();

            if (confirmMerge == "y")
            {
                contacts.Add(mergedEntry);
                changesMade = true;
                Console.WriteLine("Merged contact added.");
            }
            else
            {
                Console.WriteLine("Merge canceled.");
            }

            var toDelete = new List<Contact>();

            // Permite seleccionar cuáles contactos duplicados eliminar
            Console.WriteLine("\nSelect contacts to delete from the original duplicates:");

            for (int i = 0; i < group.Count; i++)
            {
                var c = group[i];
                Console.Write($"Delete contact [{i}] {c.firstName} {c.lastName}? [y/n]: ");
                string confirmDelete = Console.ReadLine().Trim().ToLower();

                if (confirmDelete == "y")
                {
                    toDelete.Add(c);
                }
            }

            // Elimina los contactos seleccionados
            foreach (var c in toDelete)
            {
                contacts.Remove(c);
                Console.WriteLine($"Deleted: {c.firstName} {c.lastName}");
                changesMade = true;
            }

            groupNumber++;
            Console.WriteLine();
        }

        Console.WriteLine(changesMade
            ? "Changes were made to the contact list."
            : "No changes were made to the contact list.");

        PressEnterToContinue();
    }

    // Busca y devuelve listas de contactos que se consideran duplicados
    private List<List<Contact>> FindDuplicatedContacts()
    {
        var result = new List<List<Contact>>();
        var matched = new HashSet<int>();

        for (int i = 0; i < contacts.Count; i++)
        {
            if (matched.Contains(i)) continue;

            var current = contacts[i];
            var group = new List<Contact> { current };
            var groupIndexes = new List<int> { i };

            for (int j = i + 1; j < contacts.Count; j++)
            {
                if (matched.Contains(j)) continue;

                var compare = contacts[j];

                // Compara nombre, teléfono y correo para determinar duplicados
                bool sameName = !string.IsNullOrWhiteSpace(current.firstName) &&
                                !string.IsNullOrWhiteSpace(current.lastName) &&
                                string.Equals(current.firstName, compare.firstName, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(current.lastName, compare.lastName, StringComparison.OrdinalIgnoreCase);

                bool samePhone = !string.IsNullOrWhiteSpace(current.phoneNumber) &&
                                 current.phoneNumber == compare.phoneNumber;

                bool sameEmail = !string.IsNullOrWhiteSpace(current.email) &&
                                 current.email.Equals(compare.email, StringComparison.OrdinalIgnoreCase);

                if (sameName || samePhone || sameEmail)
                {
                    group.Add(compare);
                    groupIndexes.Add(j);
                }
            }

            // Si hay más de un contacto, es un grupo de duplicados
            if (group.Count > 1)
            {
                foreach (var idx in groupIndexes)
                    matched.Add(idx);

                result.Add(group);
            }
        }

        return result.Count > 0 ? result : null;
    }

    // Permite al usuario elegir un valor para un campo específico de una lista de opciones
    private string AskUserToPickField(string fieldName, List<string> options)
    {
        // Filtra opciones vacías y duplica sólo valores únicos
        options = options.Where(opt => !string.IsNullOrWhiteSpace(opt)).Distinct().ToList();

        if (options.Count == 0) return "";

        if (options.Count == 1) return options[0];

        // Muestra las opciones disponibles para el campo
        Console.WriteLine($"\nChoose {fieldName}:");
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"[{i}] {options[i]}");
        }

        // Solicita al usuario seleccionar la opción preferida
        int choice = GetOption($"Select the preferred {fieldName}: ", 0, options.Count - 1);
        return options[choice];
    }

    // Pregunta al usuario si desea salir del programa y actúa en consecuencia
    public bool Exit()
    {
        string answer = GetOption("Are you sure that you want to exit from the program? [y/n]", "Y", "N");
        if (answer == "Y")
        {
            Console.WriteLine("Thank you for using Contact Book, Until Next time :D");
            return true;
        }
        else
        {
            MainMenu();
            return false;
        }
    }

    // Pausa la ejecución hasta que el usuario presione ENTER
    public void PressEnterToContinue()
    {
        Console.Write("Press ENTER to continue.");
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;
        Console.Clear();
    }

    // Lee y valida una opción numérica dentro de un rango
    private int GetOption(string prompt, int min, int max)
    {
        Console.Write(prompt);
        string input = Console.ReadLine().Trim();
        int option;
        while (!int.TryParse(input, out option) || option < min || option > max)
        {
            Console.WriteLine("ERROR: Invalid option.");
            Console.Write(prompt);
            input = Console.ReadLine().Trim();
        }
        return option;
    }

    // Lee y valida una opción textual entre las permitidas (case-insensitive)
    private string GetOption(string prompt, params string[] validOptions)
    {
        string option;
        do
        {
            Console.Write(prompt);
            option = Console.ReadLine().Trim().ToUpper();

            if (!validOptions.Contains(option))
            {
                Console.WriteLine("ERROR: Invalid option.");
            }

        } while (!validOptions.Contains(option));

        return option;
    }
}