using System.Net.Http;
using System.Net.Http.Json;
using HelpDeskUI.Models;

namespace HelpDeskUI;

public partial class Form1 : Form
{
    private readonly HttpClient _http = new HttpClient();
    private const string ApiUrl = "http://localhost:5000/api/requests";

    // Элементы формы
    private DataGridView grid;
    private TextBox txtTitle, txtName, txtEmail, txtDesc;
    private ComboBox cbPriority;
    private Button btnCreate, btnRefresh, btnDelete, btnStatus;
    private Label lblInfo;

    public Form1()
    {
        // Настройка формы
        this.Text = "HelpDesk";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Таблица
        grid = new DataGridView();
        grid.Location = new Point(20, 200);
        grid.Size = new Size(700, 400);
        grid.ReadOnly = true;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AllowUserToAddRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        grid.Columns.Add("Id", "ID");
        grid.Columns["Id"]!.Visible = false;
        grid.Columns.Add("Num", "Номер");
        grid.Columns.Add("Title", "Тема");
        grid.Columns.Add("Author", "Автор");
        grid.Columns.Add("Status", "Статус");
        grid.Columns.Add("Priority", "Приоритет");

        // Панель создания
        var group = new GroupBox();
        group.Text = "Новое обращение";
        group.Location = new Point(20, 20);
        group.Size = new Size(700, 160);

        // Поля
        Label lblTitle = new Label() { Text = "Тема:", Location = new Point(20, 35), Size = new Size(50, 25) };
        txtTitle = new TextBox() { Location = new Point(80, 32), Size = new Size(250, 25) };

        Label lblName = new Label() { Text = "Имя:", Location = new Point(20, 70), Size = new Size(50, 25) };
        txtName = new TextBox() { Location = new Point(80, 67), Size = new Size(150, 25) };

        Label lblEmail = new Label() { Text = "Email:", Location = new Point(250, 70), Size = new Size(50, 25) };
        txtEmail = new TextBox() { Location = new Point(310, 67), Size = new Size(200, 25) };

        Label lblPriority = new Label() { Text = "Приоритет:", Location = new Point(20, 105), Size = new Size(70, 25) };
        cbPriority = new ComboBox() { Location = new Point(100, 102), Size = new Size(100, 25) };
        cbPriority.Items.AddRange(new[] { "Low", "Normal", "High", "Critical" });
        cbPriority.SelectedIndex = 1;

        Label lblDesc = new Label() { Text = "Описание:", Location = new Point(250, 105), Size = new Size(70, 25) };
        txtDesc = new TextBox() { Location = new Point(330, 102), Size = new Size(300, 50), Multiline = true };

        btnCreate = new Button() { Text = "Создать", Location = new Point(550, 120), Size = new Size(130, 30) };
        btnCreate.Click += async (s, e) => await CreateRequest();

        group.Controls.AddRange(new Control[] { lblTitle, txtTitle, lblName, txtName, lblEmail, txtEmail,
            lblPriority, cbPriority, lblDesc, txtDesc, btnCreate });

        // Кнопки действий
        btnRefresh = new Button() { Text = "Обновить", Location = new Point(20, 170), Size = new Size(100, 30) };
        btnRefresh.Click += async (s, e) => await LoadRequests();

        btnStatus = new Button() { Text = "Сменить статус", Location = new Point(130, 170), Size = new Size(120, 30) };
        btnStatus.Click += async (s, e) => await ChangeStatus();

        btnDelete = new Button() { Text = "Удалить", Location = new Point(260, 170), Size = new Size(100, 30) };
        btnDelete.Click += async (s, e) => await DeleteRequest();

        Button btnResolve = new Button() { Text = "Решить", Location = new Point(370, 170), Size = new Size(100, 30) };
        btnResolve.Click += async (s, e) => await ResolveRequest();

        lblInfo = new Label() { Text = "Готово", Location = new Point(20, 620), Size = new Size(700, 30) };

        // Добавляем на форму
        this.Controls.AddRange(new Control[] { group, grid, btnRefresh, btnStatus, btnDelete, btnResolve, lblInfo });

        // Загружаем данные при старте
        this.Load += async (s, e) => await LoadRequests();
    }

    private async Task LoadRequests()
    {
        try
        {
            lblInfo.Text = "Загрузка...";
            var list = await _http.GetFromJsonAsync<List<SupportRequest>>(ApiUrl);

            grid.Rows.Clear();

            if (list != null)
            {
                foreach (var r in list)
                {
                    string statusText = r.Status.ToString();
                    if (r.Status == RequestStatus.New) statusText = "Новая";
                    if (r.Status == RequestStatus.InProgress) statusText = "В работе";
                    if (r.Status == RequestStatus.Resolved) statusText = "Решена";
                    if (r.Status == RequestStatus.Cancelled) statusText = "Отменена";

                    grid.Rows.Add(r.Id.ToString(), r.RequestNumber, r.Title, r.ClientName, statusText, r.Priority);
                }
            }

            lblInfo.Text = $"Всего: {grid.Rows.Count} заявок";
        }
        catch (Exception ex)
        {
            lblInfo.Text = "Ошибка: API не запущен";
            MessageBox.Show($"Ошибка: {ex.Message}\n\nЗапустите API на {ApiUrl}", "Ошибка");
        }
    }

    private async Task CreateRequest()
    {
        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            MessageBox.Show("Введите тему");
            return;
        }

        var req = new SupportRequest
        {
            Title = txtTitle.Text,
            Description = txtDesc.Text,
            ClientName = txtName.Text,
            ClientEmail = txtEmail.Text,
            Priority = cbPriority.Text
        };

        try
        {
            var response = await _http.PostAsJsonAsync(ApiUrl, req);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Заявка создана!");
                txtTitle.Clear();
                txtDesc.Clear();
                txtName.Clear();
                txtEmail.Clear();
                await LoadRequests();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
        }
    }

    private async Task ChangeStatus()
    {
        if (grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите заявку");
            return;
        }

        var id = grid.SelectedRows[0].Cells[0].Value?.ToString();
        if (string.IsNullOrEmpty(id)) return;

        var dialog = new Form();
        dialog.Text = "Выберите статус";
        dialog.Size = new Size(250, 150);
        dialog.StartPosition = FormStartPosition.CenterParent;

        var combo = new ComboBox() { Location = new Point(20, 20), Size = new Size(180, 25) };
        combo.Items.AddRange(new[] { "New", "InProgress", "Resolved", "Cancelled" });
        combo.SelectedIndex = 0;

        var btn = new Button() { Text = "OK", Location = new Point(80, 60), Size = new Size(80, 30) };
        btn.Click += async (s, e) =>
        {
            await _http.PutAsync($"{ApiUrl}/{id}/status?status={combo.Text}", null);
            await LoadRequests();
            dialog.Close();
        };

        dialog.Controls.Add(combo);
        dialog.Controls.Add(btn);
        dialog.ShowDialog();
    }

    private async Task ResolveRequest()
    {
        if (grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите заявку");
            return;
        }

        var id = grid.SelectedRows[0].Cells[0].Value?.ToString();
        if (string.IsNullOrEmpty(id)) return;

        await _http.PutAsync($"{ApiUrl}/{id}/status?status=Resolved", null);
        await LoadRequests();
        MessageBox.Show("Заявка решена!");
    }

    private async Task DeleteRequest()
    {
        if (grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите заявку");
            return;
        }

        var id = grid.SelectedRows[0].Cells[0].Value?.ToString();
        if (string.IsNullOrEmpty(id)) return;

        var result = MessageBox.Show("Удалить заявку?", "Подтверждение", MessageBoxButtons.YesNo);
        if (result == DialogResult.Yes)
        {
            await _http.DeleteAsync($"{ApiUrl}/{id}");
            await LoadRequests();
        }
    }
}