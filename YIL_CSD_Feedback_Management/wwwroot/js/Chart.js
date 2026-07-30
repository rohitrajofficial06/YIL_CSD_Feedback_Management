@section Scripts
{
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
}

<script>

    const regions = [

    @foreach(var item in Model.Regions)
    {
        <text>"@item.Region",</text>
    }

    ];

    const completion = [

    @foreach(var item in Model.Regions)
    {
        <text>@item.CompletionPercentage,</text>
    }

    ];

    const ratings = [

    @foreach(var item in Model.Regions)
    {
        <text>@item.AverageRating,</text>
    }

    ];

</script>

