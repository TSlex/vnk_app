import { DataType } from "./Enums/DataType";

export interface AttributeTypeGetDTO {
  name: string;
  usedCount: number;
  dataType: DataType;
  systemicType: boolean;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
  id: number;
}
