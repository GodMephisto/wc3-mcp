---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/SecondaryTagSequenceComparator.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x;

import java.util.Comparator;
import java.util.EnumSet;

public class SecondaryTagSequenceComparator implements Comparator<IndexedSequence> {
    private final StandSequenceComparator standSequenceComparator;
    private EnumSet<AnimationTokens.SecondaryTag> ignoredTags;

    public SecondaryTagSequenceComparator(StandSequenceComparator standSequenceComparator) {
        this.standSequenceComparator = standSequenceComparator;
    }

    public SecondaryTagSequenceComparator reset(EnumSet<AnimationTokens.SecondaryTag> ignoredTags) {
        this.ignoredTags = ignoredTags;
        return this;
    }

    @Override
    public int compare(final IndexedSequence a, final IndexedSequence b) {
        EnumSet<AnimationTokens.SecondaryTag> secondaryTagsA = a.sequence.getSecondaryTags();
        EnumSet<AnimationTokens.SecondaryTag> secondaryTagsB = b.sequence.getSecondaryTags();
        int secondaryTagsAOrdinal = getTagsOrdinal(secondaryTagsA, ignoredTags);
        int secondaryTagsBOrdinal = getTagsOrdinal(secondaryTagsB, ignoredTags);
        if (secondaryTagsAOrdinal != secondaryTagsBOrdinal) {
            return secondaryTagsBOrdinal - secondaryTagsAOrdinal;
        }
        return standSequenceComparator.compare(a, b);
    }

    public static int getTagsOrdinal(EnumSet<AnimationTokens.SecondaryTag> secondaryTagsA, EnumSet<AnimationTokens.SecondaryTag> ignoredTags) {
        int secondaryTagsBOrdinal = Integer.MAX_VALUE;
        for (AnimationTokens.SecondaryTag secondaryTag : secondaryTagsA) {
            if (secondaryTag.ordinal() < secondaryTagsBOrdinal && !ignoredTags.contains(secondaryTag)) {
                secondaryTagsBOrdinal = secondaryTag.ordinal();
            }
        }
        return secondaryTagsBOrdinal;
    }

}
