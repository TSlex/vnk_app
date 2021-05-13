<template>
  <v-row justify="center" class="text-center" v-if="loaded && attribute != null">
    <v-col cols="4" class="mt-4">
      <v-sheet rounded="lg" class="py-2">
        <div class="px-3 mb-2">
          <div class="d-flex justify-space-between mb-2">
            <span>Название атрибута:</span><span>{{ attribute.name }}</span>
          </div>
          <div class="d-flex justify-space-between mb-2">
            <span>Тип атрибута:</span
            ><v-chip small @click="onNavigateToType">{{
              attribute.type
            }}</v-chip>
          </div>
          <div class="d-flex justify-space-between mb-2">
            <span>Формат:</span
            ><v-chip small>{{ attribute.dataType | formatDataType }}</v-chip>
          </div>
          <div class="d-flex justify-space-between mb-2">
            <span>Значение по умолчанию:</span
            ><template>
              <span v-if="isDateFormat" class="text-body-1">{{
                attribute.defaultValue | formatDate
              }}</span>
              <span v-else-if="isDateTimeFormat" class="text-body-1">{{
                attribute.defaultValue | formatDateTime
              }}</span>
              <span v-else-if="isBooleanFormat" class="text-body-1">{{
                attribute.defaultValue | formatBoolean
              }}</span>
              <span v-else class="text-body-1">{{
                attribute.defaultValue
              }}</span>
            </template>
          </div>
          <div
            class="d-flex justify-space-between mb-2"
            v-if="attribute.defaultUnit"
          >
            <span>Единица измерения по умолчанию:</span
            ><span>{{ attribute.defaultUnit }}</span>
          </div>
          <div class="d-flex justify-space-between mb-2">
            <span>Количество использований:</span
            ><span>{{ attribute.usedCount }}</span>
          </div>
        </div>
        <v-divider></v-divider>
        <v-container>
          <v-btn outlined text class="mr-1" @click="onEdit">Изменить</v-btn>
          <v-btn outlined text @click="onDelete">Удалить</v-btn>
        </v-container>
      </v-sheet>
      <AttributeDeleteDialog v-model="deleteDialog" v-if="deleteDialog" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { DataType } from "~/models/Enums/DataType";
import { attributesStore } from "~/store";
import AttributeDeleteDialog from "~/components/attributes/AttributeDeleteDialog.vue";

@Component({
  components: {
    AttributeDeleteDialog,
  },
})
export default class extends Vue {
  id!: number;
  loaded = false;
  deleteDialog = false;

  get attribute() {
    return attributesStore.selectedAttribute;
  }

  get isBooleanFormat() {
    return this.attribute!.dataType === DataType.Boolean;
  }

  get isDateFormat() {
    return this.attribute!.dataType === DataType.Date;
  }

  get isDateTimeFormat() {
    return this.attribute!.dataType === DataType.DateTime;
  }

  onNavigateToType() {
    this.$router.push(`/types/${this.attribute?.typeId}`);
  }

  onEdit() {
    this.$router.push(`/attributes/edit/${this.id}`);
  }

  onDelete() {
    this.deleteDialog = true;
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

    attributesStore.getAttribute(this.id).then((suceeded) => {
      if (!suceeded) {
        this.$router.back();
      } else {
        this.loaded = true;
      }
    });
  }
}
</script>
