<template>
  <v-row justify="center" class="text-center">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Изменить тип атрибута</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-alert dense text type="error" v-if="showError">
                {{ error }}
              </v-alert>
              <v-text-field
                label="Название"
                required
                :rules="rules.name"
                v-model="model.name"
              ></v-text-field>
              <v-select
                label="Тип данных"
                required
                :items="dataTypes"
                v-model="model.dataType"
                :rules="rules.type"
              ></v-select>
              <CustomValueField
                :dataType="model.dataType"
                v-model="model.defaultCustomValue"
                :label="`Значение по умолчанию`"
                v-if="!model.usesDefinedValues"
              />
            </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click.stop="onCancel()"
              >Отмена</v-btn
            >
            <v-btn color="blue darken-1" text type="submit">Создать</v-btn>
            <v-spacer></v-spacer>
          </v-card-actions>
        </v-card>
      </v-form>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import { DataType } from "~/types/Enums/DataType";
import CustomValueField from "~/components/common/CustomValueField.vue";
import { AttributeTypePatchDTO } from "~/types/AttributeTypeDTO";

@Component({
  components: {
    CustomValueField,
  },
})
export default class AttributeTypesEdit extends Vue {
  model: AttributeTypePatchDTO = {
    id: 0,
    name: "",
    defaultCustomValue: "",
    dataType: DataType.String,
    defaultValueId: 0,
    defaultUnitId: 0,
  };

  showError = false;

  id!: number;

  get attributeType() {
    return attributeTypesStore.selectedAttributeType;
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributeTypesStore.updateAttributeType(this.model).then((suceeded) => {
        if (suceeded) {
          this.onCancel();
        } else {
          this.showError = true;
        }
      });
    }
  }

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  mounted() {
    if (!this.id) {
      this.$router.back();
    }

    attributeTypesStore.getAttributeType(this.id).then((suceeded) => {
      if (!suceeded) {
        this.$router.back();
      }
    });
  }
}
</script>
