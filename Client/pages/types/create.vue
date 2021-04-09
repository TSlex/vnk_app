<template>
  <v-row justify="center" class="text-center">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Создать тип атрибута</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-alert dense text type="error" v-if="showError">
                {{ error }}
              </v-alert>
              <v-text-field label="Название" required></v-text-field>
              <v-select
                label="Тип данных"
                required
                :items="dataTypes"
                v-model="model.dataType"
              ></v-select>
              <CustomValueField
                :dataType="model.dataType"
                v-model="model.defaultCustomValue"
                :label="`Значение по умолчанию`"
              />
              <v-switch label="Значения определены" inset></v-switch>
              <v-switch label="Единицы определены" inset></v-switch>
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
import { localize } from "~/utils/localizeDataType";
import CustomValueField from "~/components/common/CustomValueField.vue";
import { AttributeTypePostDTO } from "~/types/AttributeTypeDTO";

@Component({
  components: {
    CustomValueField,
  },
})
export default class AttributeTypesCreate extends Vue {
  model: AttributeTypePostDTO = {
    name: "",
    defaultCustomValue: "",
    dataType: DataType.String,
    usesDefinedValues: false,
    usesDefinedUnits: false,
    defaultValueIndex: 0,
    defaultUnitIndex: 0,
    values: [],
    units: [],
  };

  showError = false;

  get error() {
    return attributeTypesStore.error;
  }

  get dataTypes() {
    let types: any = [];

    Object.values(DataType).forEach((element) => {
      let key = Number(element);
      if (!isNaN(Number(element)) && key >= 0) {
        types.push({
          text: localize(element as DataType),
          value: element as DataType,
        });
      }
    });

    return types;
    // return [
    //   { text: localize(DataType.Boolean), value: DataType.Boolean },
    //   { text: localize(DataType.String), value: DataType.Boolean },
    //   { text: localize(DataType.Integer), value: DataType.Boolean },
    //   { text: localize(DataType.Float), value: DataType.Boolean },
    //   { text: localize(DataType.), value: DataType.Boolean },
    //   { text: localize(DataType.Boolean), value: DataType.Boolean },
    //   { text: localize(DataType.Boolean), value: DataType.Boolean },
    // ];
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    // this.$router.back()
    if ((this.$refs.form as any).validate()) {
      console.log(this.model.defaultCustomValue);
    }
  }
}
</script>
