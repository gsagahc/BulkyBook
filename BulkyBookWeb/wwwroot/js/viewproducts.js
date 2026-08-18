var productDataTable;
new DataTable('#tblData', {
            ajax: '/admin/product/getall?includeCategory=true',
        columns: [
        { data: 'title', "width": "25%" },
        { data: 'isbn', "width": "15%" },
        { data: 'price', "width": "10%", "render": function (data) { return 'R$' + data.toFixed(2); } },
        { data: 'author', "width": "15%" },
        {
            data: 'category.name', "width": "10%", "render": function (data) { return '<span class="badge bg-secondary">' + data + '</span>'; }
        },
       
    ]
});
