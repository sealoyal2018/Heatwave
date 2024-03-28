<template>
  <div class="main">
    <el-form ref="searchFormRef" :inline="true" :model="formState"
      class="search-form bg-bg_color w-[99/100] pl-8 pt-[12px] overflow-auto">
      <el-form-item label="应用名称" prop="name">
        <el-input v-model="formState.name" placeholder="请输入应用名称" clearable class="!w-[180px]"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" :icon="useRenderIcon('ri:search-line')" :loading="loading"
          @click="handleSearch">搜索</el-button>
        <el-button :icon="useRenderIcon(Refresh)" @click="handleReset(searchFormRef)">重置</el-button>
      </el-form-item>
    </el-form>
    <pure-table ref="tableRef" border adaptive alignWhole="center" showOverflowTooltip :loading="loading"
      :data="dataList" :columns="columns" :pagination="pagination" @page-current-change="handleCurrentChange"
      @page-size-change="handlePageSizeChange">
      <template #operation="{ row }">
        <el-button class="reset-margin" link type="primary" :icon="useRenderIcon(EditPen)">
          修改
        </el-button>
        <el-popconfirm :title="`是否确认删除角色名称为${row.name}的这条数据`" @confirm="handleDelete(row)">
          <template #reference>
            <el-button class="reset-margin" link type="primary" :size="size" :icon="useRenderIcon(Delete)">
              删除
            </el-button>
          </template>
        </el-popconfirm>
        <!-- <el-dropdown>
              <el-button
                class="ml-3 mt-[2px]"
                link
                type="primary"
                :size="size"
                :icon="useRenderIcon(More)"
              />
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item>
                    <el-button
                      :class="buttonClass"
                      link
                      type="primary"
                      :size="size"
                      :icon="useRenderIcon(Menu)"
                      @click="handleMenu"
                    >
                      菜单权限
                    </el-button>
                  </el-dropdown-item>
                  <el-dropdown-item>
                    <el-button
                      :class="buttonClass"
                      link
                      type="primary"
                      :size="size"
                      :icon="useRenderIcon(Database)"
                      @click="handleDatabase"
                    >
                      数据权限
                    </el-button>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown> -->
      </template>
    </pure-table>
  </div>
</template>

<script setup lang="ts">

import { useRenderIcon } from '@/components/ReIcon/src/hooks';
import { reactive, ref } from 'vue';
import Refresh from "@iconify-icons/ep/refresh";
import EditPen from "@iconify-icons/ep/edit-pen";
import Delete from "@iconify-icons/ep/delete";
import { AppPlatform, getAppPlatforms } from '@/api/system/appPlatform'
import { useAppPlatform } from './hook';

const {
  formState,
  loading,
  pagination,
  columns,
  handleSearch,
  handleReset,
  handleCurrentChange,
  handlePageSizeChange,
  handleDelete,
  dataList
} = useAppPlatform();

const tableRef = ref();
const searchFormRef = ref();

</script>

<style lang="scss" scoped>
:deep(.el-table__inner-wrapper::before) {
  height: 0;
}

.main-content {
  margin: 24px 24px 0 !important;
}

.search-form {
  :deep(.el-form-item) {
    margin-bottom: 12px;
  }
}
</style>
