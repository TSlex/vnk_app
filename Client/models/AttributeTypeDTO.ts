import { PatchOption } from './Enums/PatchOption';
import { DataType } from "./Enums/DataType";

export interface AttributeTypeGetDTO {
  id: number;
  name: string;
  usedCount: number;
  dataType: DataType;
  systemicType: boolean;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
}

export interface AttributeTypeGetValueDTO {
  value: string;
  id: number;
}

export interface AttributeTypeGetUnitDTO {
  value: string;
  id: number;
}

export interface AttributeTypeDetailsGetDTO {
  name: string;
  usedCount: number;
  defaultCustomValue: string;
  dataType: number;
  systemicType: boolean;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
  defaultValueId: number;
  defaultUnitId: number;
  valuesCount: number;
  unitsCount: number;
  values: AttributeTypeGetValueDTO[];
  units: AttributeTypeGetUnitDTO[];
  id: number;
}

export interface AttributeTypePostDTO {
  name: string;
  defaultCustomValue: string;
  dataType: DataType;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
  defaultValueIndex: number;
  defaultUnitIndex: number;
  values: string[];
  units: string[];
}

export interface AttributeTypePatchDTO {
  id: number;
  name: string;
  defaultCustomValue: string;
  dataType: DataType;
  defaultValueId: number;
  defaultUnitId: number;
  values: AttributeTypeValuePatchDTO[];
  units: AttributeTypeUnitPatchDTO[];
}

export interface AttributeTypeValuePatchDTO {
  value: string;
  patchOption: PatchOption;
  id: number | null;
}

export interface AttributeTypeUnitPatchDTO {
  value: string;
  patchOption: PatchOption;
  id: number | null;
}
