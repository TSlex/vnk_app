<template>
  <v-row
    justify="center"
    class="text-center"
    v-if="loaded && attributeType != null"
  >
    <v-col cols="4" class="mt-4">
      <v-sheet rounded="lg" class="py-2">
        <div class="px-3 mb-2">
          <div class="d-flex justify-space-between mb-2">
            <span>Название:</span><span>{{ attributeType.name }}</span>
          </div>
          <div class="d-flex justify-space-between mb-2">
            <span>Формат данных:</span
            ><v-chip small>{{
              attributeType.dataType | formatDataType
            }}</v-chip>
          </div>
          <div class="d-flex justify-space-between">
            <span>Количество использований:</span
            ><span>{{ attributeType.usedCount }}</span>
          </div>
          <div
            class="d-flex justify-space-between mt-2"
            v-if="
              !attributeType.usesDefinedValues &&
              attributeType.defaultCustomValue
            "
          >
            <span>Значение по умолчанию:</span>
            <span v-if="isDateFormat" class="text-body-1">{{
              attributeType.defaultCustomValue | formatDate
            }}</span>
            <span v-else-if="isDateTimeFormat" class="text-body-1">{{
              attributeType.defaultCustomValue | formatDateTime
            }}</span>
            <span v-else-if="isBooleanFormat" class="text-body-1">{{
              attributeType.defaultCustomValue | formatBoolean
            }}</span>
            <span v-else class="text-body-1">{{
              attributeType.defaultCustomValue
            }}</span>
          </div>
        </div>
        <template v-if="!attributeType.systemicType">
          <v-divider></v-divider>
          <v-container>
            <v-btn outlined text class="mr-1" @click="onEdit">Изменить</v-btn>
            <v-btn outlined text @click="onDelete" :disabled="!deletable"
              >Удалить</v-btn
            >
          </v-container>
        </template>
      </v-sheet>
      <v-sheet rounded="lg" class="mt-4 pt-1" v-if="valuesCount > 0">
        <v-toolbar flat dense>
          <v-toolbar-title>Допустимые значения</v-toolbar-title>
        </v-toolbar>
        <v-divider></v-divider>
        <div class="px-2">
          <div
            class="d-flex justify-space-between pa-2"
            v-for="value in attributeType.values"
            :key="value.id"
          >
            <template>
              <span v-if="isDateFormat" class="text-body-1">{{
                value.value | formatDate
              }}</span>
              <span v-else-if="isDateTimeFormat" class="text-body-1">{{
                value.value | formatDateTime
              }}</span>
              <span v-else-if="isBooleanFormat" class="text-body-1">{{
                value.value | formatBoolean
              }}</span>
              <span v-else class="text-body-1">{{ value.value }}</span>
              <v-chip v-if="attributeType.defaultValueId === value.id" small
                >по умолчанию</v-chip
              >
            </template>
          </div>
        </div>
      </v-sheet>
      <v-sheet rounded="lg" class="mt-4 pt-1" v-if="unitsCount > 0">
        <v-toolbar flat dense>
          <v-toolbar-title>Единицы измерения</v-toolbar-title>
        </v-toolbar>
        <v-divider></v-divider>
        <div class="px-2">
          <div
            class="d-flex justify-space-between pa-2"
            v-for="unit in attributeType.units"
            :key="unit.id"
          >
            <template>
              <span class="text-body-1">{{ unit.value }}</span>
              <v-chip v-if="attributeType.defaultUnitId === unit.id" small
                >по умолчанию</v-chip
              >
            </template>
          </div>
        </div>
      </v-sheet>
      <TypeDeleteDialog v-model="deleteDialog" v-if="deleteDialog" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import { DataType } from "~/models/Enums/DataType";
import TypeDeleteDialog from "~/components/types/TypeDeleteDialog.vue";

@Component({
  components: {
    TypeDeleteDialog,
  },
})
export default class extends Vue {
  id!: number;
  loaded = false;
  deleteDialog = false;

  get attributeType() {
    return attributeTypesStore.selectedAttributeType;
  }

  get valuesCount() {
    return this.attributeType!.usesDefinedValues
      ? this.attributeType!.values.length
      : 0;
  }

  get unitsCount() {
    return this.attributeType!.units.length;
  }

  get isBooleanFormat() {
    return this.attributeType!.dataType === DataType.Boolean;
  }

  get isDateFormat() {
    return this.attributeType!.dataType === DataType.Date;
  }

  get isDateTimeFormat() {
    return this.attributeType!.dataType === DataType.DateTime;
  }

  get deletable() {
    return this.attributeType != null && this.attributeType.usedCount == 0;
  }

  onEdit() {
    this.$router.push(`/types/edit/${this.id}`);
  }

  onDelete() {
    if (this.deletable) {
      this.deleteDialog = true;
    }
  }

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  updated() {
    if (!this.id) {
      this.$router.back();
    }
  }

  mounted() {
    if (!this.id) {
      this.$router.back();
    }

    attributeTypesStore.getAttributeType(this.id).then((suceeded) => {
      if (!suceeded) {
        this.$router.back();
      } else {
        this.loaded = true;
      }
    });
  }
}
</script>


