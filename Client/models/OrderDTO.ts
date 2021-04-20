export interface OrderGetDTO {
  id: number;
  name: string;
  completed: boolean;
  overdued: boolean;
  notation: string;
  executionDateTime: Date | null;
  attributes: OrderAttributeGetDTO[];
}

export interface OrderAttributeGetDTO {
  id: number;
  name: string;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
  value: string;
  unit: string;
  valueId: number | null;
  unitId: number | null;
  attributeId: number;
  typeId: number;
  type: string;
  dataType: number;
  featured: boolean;
}

export interface OrderPostDTO {
  name: string;
  completed: boolean;
  notation: string | null;
  executionDateTime: Date | null;
  attributes: OrderAttributePostDTO[];
}

export interface OrderAttributePostDTO {
  featured: boolean;
  attributeId: number;
  customValue: string | null;
  valueId: number | null;
  unitId: number | null;
}

export interface OrderCompletionPatchDTO {
  id: number;
  completed: boolean;
}

export interface OrderPatchDTO {
  id: number;
  name: string;
  completed: boolean | null;
  notation: string | null;
  executionDateTime: Date | null;
  attributes: OrderAttributePatchDTO[];
}

export interface OrderAttributePatchDTO {
  id: number;
  patchOption: PatchOption;
  featured: boolean;
  attributeId: number;
  customValue: string | null;
  valueId: number | null;
  unitId: number | null;
}

export interface OrderHistoryDTO {
  id: number;
  masterId: number | null;
  createdBy: string;
  createdAt: Date;
  changedBy: string;
  changedAt: Date;
  deletedBy: string | null;
  deletedAt: Date | null;
  name: string;
  completed: boolean;
  notation: string;
  executionDateTime: Date | null;
  attributes: OrderAttributeGetDTO[];
}
