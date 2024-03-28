import { message } from "@/utils/message";
import { ref, Ref, reactive, onMounted } from "vue";
import { getAppPlatforms } from '@/api/system/appPlatform'


export function useAppPlatform() {
  const formState = reactive({
    name: '',
  });

  const loading = ref(false);
  const dataList = ref([]);
  const pagination = reactive({
    total: 0,
    pageSize: 10,
    currentPage: 1,
    background: true,
  });

  const columns: TableColumnList = [
    {
      label: '名称',
      prop: 'name',
    },
    {
      label: 'Key',
      prop: 'key'
    },
    {
      label: 'Secret',
      prop: 'secret'
    },
    {
      label: '操作',
      slot: 'operation'
    }
  ];

  function handleDelete(row) {
    message(`你确定删除名为${row.name}的应用程序`, { type: 'warning' });

  }

  function featchData() {
    loading.value = true;
    getAppPlatforms({
      size: pagination.pageSize,
      index: pagination.currentPage,
      name: formState.name
    }).then(res => {
      if (res.code === 200) {
        const { data } = res;
        pagination.total = data.totalPages;
        pagination.currentPage = data.pageNumber;
        dataList.value = data.items;
      } else {
        dataList.value = [];
        pagination.total = 0;
        pagination.currentPage = 1;
      }
      loading.value = false;
    })
      .catch(() => {
        loading.value = false;
      });
  }

  function handleSearch() {
    pagination.currentPage = 1;
    featchData();
  }

  const handleReset = (formEl) => {
    if (!formEl)
      return;
    formEl.resetFields();
    handleSearch();
  }

  const handleCurrentChange = () => {
    console.log(' page >>>>>>', pagination);
    handleSearch();
  }

  const handlePageSizeChange = () => {
    console.log(' handlePageSizeChange page >>>>>>', pagination);
    handleSearch();
  }


  onMounted(async () => {
    handleSearch();
  })

  return {
    formState,
    loading,
    pagination,
    columns,
    handleSearch,
    handleReset,
    handleDelete,
    handleCurrentChange,
    handlePageSizeChange,
    dataList
  }
}

